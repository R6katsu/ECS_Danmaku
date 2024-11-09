using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Mathematics;
using static HealthHelper;
using static PlayerHelper;
#endif

/// <summary>
/// PLの処理
/// </summary>
public partial class PlayerSystem : SystemBase
{
    // InputSystem
    private PlayerControls _playerInput;

    // 入力
    private float _horizontalValue;
    private float _verticalValue;

    [Tooltip("射撃中かのフラグ")]
    private bool _isShot;

    protected override void OnCreate()
    {
        // ShooterControlsをインスタンス化し、有効にする
        _playerInput = new PlayerControls();
        _playerInput.Enable();

        // Shotに割り当てる
        var shot = _playerInput.Player.Shot;
        shot.started += (context) => _isShot = true;
        shot.canceled += (context) => _isShot = false;

        // Horizontalに割り当てる
        var horizontal = _playerInput.Player.Horizontal;
        horizontal.started += Horizontal;
        horizontal.canceled += Horizontal;

        // Verticalに割り当てる
        var vertical = _playerInput.Player.Vertical;
        vertical.started += Vertical;
        vertical.canceled += Vertical;

        // Slowdownに割り当てる
        var slowdown = _playerInput.Player.Slowdown;
        slowdown.started += Slowdown;
        slowdown.canceled += Slowdown;
    }

    protected override void OnUpdate()
    {
        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataを持つEntityを取得
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(playerEntity)) { return; }

        // PLからLocalTransformを取得
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // フレーム秒数、経過時間
        var delta = SystemAPI.Time.DeltaTime;
        var elapsed = SystemAPI.Time.ElapsedTime;

        // 移動処理
        PlayerMove(delta, playerEntity, playerSingleton, playerTransform);

        if (_isShot)
        {
            // PlayerTagの付いたShooterで発射する
            // AutoShooterはShotを押さなくても勝手に射撃する
            // 敵についても、敵から直接発射するのではなくShooterPrefabから発射するように設計する



            // 射撃処理
            PlayerShot(elapsed, playerEntity, playerSingleton, playerTransform);
        }
    }

    protected override void OnDestroy()
    {
        _playerInput.Disable();
        _playerInput.Dispose();
    }

    /// <summary>
    /// 水平入力の処理
    /// </summary>
    /// <param name="context"></param>
    private void Horizontal(InputAction.CallbackContext context)
    {
        _horizontalValue = context.ReadValue<float>();
    }

    /// <summary>
    /// 垂直入力の処理
    /// </summary>
    /// <param name="context"></param>
    private void Vertical(InputAction.CallbackContext context)
    {
        _verticalValue = context.ReadValue<float>();
    }

    /// <summary>
    /// 減速入力の処理
    /// </summary>
    /// <param name="context"></param>
    private void Slowdown(InputAction.CallbackContext context)
    {
        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        switch (context.phase)
        {
            case InputActionPhase.Started:
                // 減速フラグを立てる
                playerSingleton.isSlowdown = true;
                break;

            case InputActionPhase.Canceled:
                // 減速フラグを折る
                playerSingleton.isSlowdown = false;
                break;
        }

        // PlayerSingletonDataを持つEntityを取得
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // 変更を反映
        SystemAPI.SetComponent(playerEntity, playerSingleton);
    }

    /// <summary>
    /// PLの移動処理
    /// </summary>
    /// <param name="delta">現在のフレーム秒数</param>
    /// <param name="playerEntity">PLのEntity</param>
    /// <param name="playerSingleton">PLのSingletonData</param>
    /// <param name="playerTransform">PLのLocalTransform</param>
    public void PlayerMove(float delta, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // MovementRangeSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // シングルトンデータの取得
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // PLの移動可能範囲を取得
        var playerMovementRange = movementRangeSingleton.playerMovementRange;

        // 中心位置
        var movementRangeCenter = playerMovementRange.movementRangeCenter;

        // 半分の大きさを求める
        var halfMovementRange = playerMovementRange.movementRange / 2;

        // 中心位置を考慮した移動可能範囲を求める
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // 現在地を取得
        var currentPosition = playerTransform.Position;

        // 移動速度
        var speed = (playerSingleton.isSlowdown) ? playerSingleton.slowMoveSpeed : playerSingleton.moveSpeed;

        // 水平入力、垂直入力からfloat3を作成
        var movementDirection = new float3(_horizontalValue, new float(), _verticalValue);

        // 入力がある場合は正規化
        movementDirection = (math.lengthsq(movementDirection) > 0) ?
                math.normalize(movementDirection) : movementDirection;

        // 移動量を算出
        Vector3 scaledMovement = movementDirection * speed * delta;

        // 移動を加算代入
        currentPosition += (float3)scaledMovement;

        // 移動可能範囲内に収める
        currentPosition.x = Mathf.Clamp(currentPosition.x, minMovementRange.x, maxMovementRange.x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minMovementRange.y, maxMovementRange.y);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minMovementRange.z, maxMovementRange.z);

        // 反映
        playerTransform.Position = currentPosition;

        // カメラ位置に反映
        GameManager.Instance.GameCameraPosition = currentPosition;

        // 変更を反映
        SystemAPI.SetComponent(playerEntity, playerTransform);
    }

    public void PlayerShot(double elapsed, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // 最後に射撃した時刻と現在の時間から経過時間を求め、射撃間隔を満たしていなければ切り上げる
        if (elapsed - playerSingleton.lastShotTime <= playerSingleton.firingInterval) { return; }

        // 最後に射撃した時刻を更新
        playerSingleton.lastShotTime = elapsed;

        // PLの弾を生成
        var playerBulletEntity = EntityManager.Instantiate(playerSingleton.playerBulletEntity);

        // 弾のLocalTransformを取得
        var localTransform = SystemAPI.GetComponent<LocalTransform>(playerBulletEntity);

        // 現在地を代入
        localTransform.Position = playerTransform.Position;

        // 変更を適用
        SystemAPI.SetComponent(playerBulletEntity, localTransform);
        SystemAPI.SetComponent(playerEntity, playerSingleton);
    }
}
