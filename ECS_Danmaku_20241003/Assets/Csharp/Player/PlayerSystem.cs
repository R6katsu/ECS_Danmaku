using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

#if UNITY_EDITOR
using static UnityEngine.Rendering.DebugUI;
using Unity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.SocialPlatforms.Impl;
using static HealthHelper;
using static PlayerHelper;
#endif

// リファクタリング済み

/// <summary>
/// PLの処理
/// </summary>
public partial class PlayerSystem : SystemBase
{
    // InputSystem
    private PlayerControls _playerInput = null;

    // 入力
    private float _horizontalValue = 0.0f;
    private float _verticalValue = 0.0f;

    [Tooltip("溜め攻撃中")]
    private bool _isChargeShot = false;

    [Tooltip("減速中")]
    private bool _isSlowdown = false;

    [Tooltip("最後に攻撃が当たった時間")]
    private double _lastShotTime = 0.0f;

    [Tooltip("現在のチャージ時間")]
    private float _currentChargeTime = 0.0f;

    /// <summary>
    /// 現在のチャージ時間
    /// </summary>
    private float CurrentChargeTime
    {
        get => _currentChargeTime;
        set
        {
            _currentChargeTime = value;

            if (_currentChargeTime <= 0)
            {
                // チャージ最大
                // いや、離したら0になるって感じでいいのでは？
                // チャージ時間がn以上だった場合はチャージ完了のため離したら溜め攻撃！でいいか
                // 段階分けできたらする
            }
        }
    }

    protected override void OnCreate()
    {
        // ShooterControlsをインスタンス化し、有効にする
        // リソース解放の為にフィールド変数として保持する
        _playerInput = new PlayerControls();
        _playerInput.Enable();

        // ChargeShotに割り当てる
        var chargeShot = _playerInput.Player.ChargeShot;
        chargeShot.started += (context) => _isChargeShot = true;
        chargeShot.canceled += (context) => _isChargeShot = false;

        // Horizontalに割り当てる
        var horizontal = _playerInput.Player.Horizontal;
        horizontal.started += Horizontal;
        horizontal.started += TiltOnMoveStart;
        horizontal.canceled += Horizontal;
        horizontal.canceled += TiltOnMoveEnd;

        // Verticalに割り当てる
        var vertical = _playerInput.Player.Vertical;
        vertical.started += Vertical;
        vertical.canceled += Vertical;

        // Slowdownに割り当てる
        var slowdown = _playerInput.Player.Slowdown;
        slowdown.started += (context) => _isSlowdown = true;
        slowdown.canceled += (context) => _isSlowdown = false;
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

        var delta = SystemAPI.Time.DeltaTime;       // フレーム秒数
        var elapsed = SystemAPI.Time.ElapsedTime;   // 経過時間

        // 移動処理
        PlayerMove(delta, playerEntity, playerSingleton, playerTransform);

        if (_isChargeShot)
        {
            _currentChargeTime += delta;
            //chargeTime
        }
        else
        {
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
    /// PLの移動処理
    /// </summary>
    /// <param name="delta">現在のフレーム秒数</param>
    /// <param name="playerEntity">PLのEntity</param>
    /// <param name="playerSingleton">PLのSingletonData</param>
    /// <param name="playerTransform">PLのLocalTransform</param>
    public void PlayerMove(float delta, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // 入力がなかった
        if (_verticalValue == 0.0f && _horizontalValue == 0.0f) { return; }

        // MovementRangeSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // シングルトンデータの取得
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // PLの移動可能範囲を取得
        var playerMovementRange = movementRangeSingleton.PlayerMovementRange;

        // 中心位置
        var movementRangeCenter = playerMovementRange.movementRangeCenter;

        // 半分の大きさを求める
        var halfMovementRange = playerMovementRange.movementRange.Halve();

        // 中心位置を考慮した移動可能範囲を求める
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // 現在地を取得
        var currentPosition = playerTransform.Position;

        // 移動速度
        var speed = (_isSlowdown) ? playerSingleton.slowMoveSpeed : playerSingleton.moveSpeed;

        // 水平入力、垂直入力からfloat3を作成
        var movementDirection = new float3(_horizontalValue, 0.0f, _verticalValue);

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

        // 変更を反映
        SystemAPI.SetComponent(playerEntity, playerTransform);
    }

    /// <summary>
    /// PLの通常攻撃
    /// </summary>
    /// <param name="elapsed">経過時間</param>
    /// <param name="playerEntity">PLEntity</param>
    /// <param name="playerSingleton">PLData</param>
    /// <param name="playerTransform">PLTfm</param>
    public void PlayerShot(double elapsed, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // 最後に射撃した時刻と現在の時間から経過時間を求め、射撃間隔を満たしていなければ切り上げる
        if (elapsed - _lastShotTime <= playerSingleton.firingInterval) { return; }

        // 最後に射撃した時刻を更新
        _lastShotTime = elapsed;

        // PLの弾を生成
        var playerBulletEntity = EntityManager.Instantiate(playerSingleton.playerBulletEntity);

        // 弾のLocalTransformを取得
        var bulletTfm = SystemAPI.GetComponent<LocalTransform>(playerBulletEntity);

        // 現在地を代入
        bulletTfm.Position = playerTransform.Position;

        // 現在の回転を代入
        bulletTfm.Rotation = playerTransform.Rotation;

        // 変更を適用
        SystemAPI.SetComponent(playerBulletEntity, bulletTfm);
        SystemAPI.SetComponent(playerEntity, playerSingleton);
    }

    /// <summary>
    /// 移動時に体を傾ける処理を開始
    /// </summary>
    /// <param name="context"></param>
    private void TiltOnMoveStart(InputAction.CallbackContext context)
    {
        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataが持つEntityを取得
        Entity modelEntity = playerSingleton.playerModelEntity;

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(modelEntity)) { return; }

        // ModelからLocalTransformを取得
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(modelEntity);

        // 1、0、-1の整数に変換
        var dir = (int)math.sign(_horizontalValue);

        // 変更を適用
        playerTransform.Rotation = quaternion.EulerXYZ(0, math.radians(playerSingleton.playerMoveTilt * dir), 0);
        SystemAPI.SetComponent(modelEntity, playerTransform);
    }

    /// <summary>
    /// 移動時に体を傾ける処理を終了
    /// </summary>
    /// <param name="context"></param>
    public void TiltOnMoveEnd(InputAction.CallbackContext context)
    {
        // PlayerSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // シングルトンデータの取得
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonDataが持つEntityを取得
        Entity modelEntity = playerSingleton.playerModelEntity;

        // LocalTransformを所持していなかった
        if (!SystemAPI.HasComponent<LocalTransform>(modelEntity)) { return; }

        // ModelからLocalTransformを取得
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(modelEntity);

        // 傾きを初期化
        playerTransform.Rotation = Quaternion.identity;

        // 変更を適用
        SystemAPI.SetComponent(modelEntity, playerTransform);
    }
}
