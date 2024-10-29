using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using static HealthHelper;
using static PlayerHelper;

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
        var delta = SystemAPI.Time.DeltaTime;
        var elapsed = SystemAPI.Time.ElapsedTime;

        // 移動処理
        foreach (var (playerTag, playerData, localTfm) in
            SystemAPI.Query<
                RefRO<PlayerTag>, 
                RefRO<PlayerData>, 
                RefRW<LocalTransform>>())
        {
            // 現在地を取得
            var currentPosition = localTfm.ValueRO.Position;

            // 移動速度
            var speed = (playerData.ValueRO.isSlowdown) ? playerData.ValueRO.slowMoveSpeed : playerData.ValueRO.moveSpeed;

            // 水平入力を水平方向に加算代入
            currentPosition.x += _horizontalValue * speed * delta;

            // 垂直入力を垂直方向に加算代入
            currentPosition.z += _verticalValue * speed * delta;

            // 反映
            localTfm.ValueRW.Position = currentPosition;

            // カメラ位置に反映
            GameManager.Instance.GameCameraPosition = currentPosition;
        }

        if (_isShot)
        {
            // PlayerTagの付いたShooterで発射する
            // AutoShooterはShotを押さなくても勝手に射撃する
            // 敵についても、敵から直接発射するのではなくShooterPrefabから発射するように設計する



            // 射撃処理
            foreach (var (playerTag, playerData, localTfm) in
                SystemAPI.Query<
                    RefRO<PlayerTag>,
                    RefRW<PlayerData>,
                    RefRW<LocalTransform>>())
            {
                // 最後に射撃した時刻と現在の時間から経過時間を求め、射撃間隔を満たしていなければコンテニュー
                if (elapsed - playerData.ValueRW.lastShotTime <= playerData.ValueRO.firingInterval) { continue; }

                // 最後に射撃した時刻を更新
                playerData.ValueRW.lastShotTime = elapsed;

                // PLの弾を生成
                var playerBulletEntity = EntityManager.Instantiate(playerData.ValueRO.playerBulletEntity);

                // 弾のLocalTransformを取得
                var localTransform = SystemAPI.GetComponent<LocalTransform>(playerBulletEntity);

                // 現在地を代入
                localTransform.Position = localTfm.ValueRO.Position;

                // 変更を適用
                SystemAPI.SetComponent(playerBulletEntity, localTransform);
            }
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
        Debug.Log("そもそもPLは一体しか居ないのだからシングルトンでいいのでは？");

        switch (context.phase)
        {
            case InputActionPhase.Started:
                // 減速処理
                foreach (var (playerTag, playerData, localTfm) in
                    SystemAPI.Query<
                        RefRO<PlayerTag>,
                        RefRW<PlayerData>,
                        RefRW<LocalTransform>>())
                {
                    playerData.ValueRW.isSlowdown = true;
                }
                break;

            case InputActionPhase.Canceled:
                // 減速処理
                foreach (var (playerTag, playerData, localTfm) in
                    SystemAPI.Query<
                        RefRO<PlayerTag>,
                        RefRW<PlayerData>,
                        RefRW<LocalTransform>>())
                {
                    playerData.ValueRW.isSlowdown = false;
                }
                break;
        }
    }
}
