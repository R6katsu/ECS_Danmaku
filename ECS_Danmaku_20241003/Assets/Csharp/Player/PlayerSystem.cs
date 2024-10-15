using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerHelper;

/// <summary>
/// PL�̏���
/// </summary>
public partial class PlayerSystem : SystemBase
{
    // InputSystem
    private PlayerControls _playerInput;

    // ����
    private float _horizontalValue;
    private float _verticalValue;

    protected override void OnCreate()
    {
        // ShooterControls���C���X�^���X�����A�L���ɂ���
        _playerInput = new PlayerControls();
        _playerInput.Enable();

        // Shot�Ɋ��蓖�Ă�
        var shot = _playerInput.Player.Shot;
        shot.started += Shot;

        // Horizontal�Ɋ��蓖�Ă�
        var horizontal = _playerInput.Player.Horizontal;
        horizontal.started += Horizontal;
        horizontal.canceled += Horizontal;

        // Vertical�Ɋ��蓖�Ă�
        var vertical = _playerInput.Player.Vertical;
        vertical.started += Vertical;
        vertical.canceled += Vertical;

        // AileronRoll�Ɋ��蓖�Ă�
        var aileronRoll = _playerInput.Player.AileronRoll;
        aileronRoll.started += AileronRoll;
        aileronRoll.canceled += AileronRoll;
    }

    protected override void OnUpdate()
    {
        var delta = SystemAPI.Time.DeltaTime;

        // �ړ�����
        foreach (var (playerTag, playerData, localTfm) in
            SystemAPI.Query<
                RefRO<PlayerTag>, 
                RefRO<PlayerData>, 
                RefRW<LocalTransform>>())
        {
            // ���ݒn���擾
            var currentPosition = localTfm.ValueRO.Position;

            // �ړ����x
            var speed = delta * playerData.ValueRO.moveSpeed;

            // �������͂𐅕������ɉ��Z���
            currentPosition.x += _horizontalValue * speed;

            // �������͂𐂒������ɉ��Z���
            currentPosition.z += _verticalValue * speed;

            // ���f
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
        Debug.Log("������");
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
    /// �@�̂𐂒��ɌX�����z�̋쓮�B�ړ����x���x���Ȃ�A�e��������₷���Ȃ�B
    /// ���̏�A��e�ʐς�����B���̎��͍U���ł��Ȃ��B
    /// </summary>
    /// <param name="context"></param>
    private void AileronRoll(InputAction.CallbackContext context)
    {
        Debug.Log("������");
    }
}
