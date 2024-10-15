using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
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

    protected override void OnCreate()
    {
        // ShooterControlsをインスタンス化し、有効にする
        _playerInput = new PlayerControls();
        _playerInput.Enable();

        // Shotに割り当てる
        var shot = _playerInput.Player.Shot;
        shot.started += Shot;

        // Horizontalに割り当てる
        var horizontal = _playerInput.Player.Horizontal;
        horizontal.started += Horizontal;
        horizontal.canceled += Horizontal;

        // Verticalに割り当てる
        var vertical = _playerInput.Player.Vertical;
        vertical.started += Vertical;
        vertical.canceled += Vertical;

        // AileronRollに割り当てる
        var aileronRoll = _playerInput.Player.AileronRoll;
        aileronRoll.started += AileronRoll;
        aileronRoll.canceled += AileronRoll;
    }

    protected override void OnUpdate()
    {
        var delta = SystemAPI.Time.DeltaTime;

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
            var speed = delta * playerData.ValueRO.moveSpeed;

            // 水平入力を水平方向に加算代入
            currentPosition.x += _horizontalValue * speed;

            // 垂直入力を垂直方向に加算代入
            currentPosition.z += _verticalValue * speed;

            // 反映
            localTfm.ValueRW.Position = currentPosition;
        }
    }

    protected override void OnDestroy()
    {
        _playerInput.Disable();
        _playerInput.Dispose();
    }

    private void Shot(InputAction.CallbackContext context)
    {
        Debug.Log("未実装");
    }

    private void Horizontal(InputAction.CallbackContext context)
    {
        _horizontalValue = context.ReadValue<float>();
    }

    private void Vertical(InputAction.CallbackContext context)
    {
        _verticalValue = context.ReadValue<float>();
    }

    /// <summary>
    /// 機体を垂直に傾ける空想の駆動。移動速度が遅くなり、弾幕を避けやすくなる。
    /// その上、被弾面積が減る。この時は攻撃できない。
    /// </summary>
    /// <param name="context"></param>
    private void AileronRoll(InputAction.CallbackContext context)
    {
        Debug.Log("未実装");
    }
}
