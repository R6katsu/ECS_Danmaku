using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using static HealthHelper;
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

    [Tooltip("�ˌ������̃t���O")]
    private bool _isShot;

    protected override void OnCreate()
    {
        // ShooterControls���C���X�^���X�����A�L���ɂ���
        _playerInput = new PlayerControls();
        _playerInput.Enable();

        // Shot�Ɋ��蓖�Ă�
        var shot = _playerInput.Player.Shot;
        shot.started += (context) => _isShot = true;
        shot.canceled += (context) => _isShot = false;

        // Horizontal�Ɋ��蓖�Ă�
        var horizontal = _playerInput.Player.Horizontal;
        horizontal.started += Horizontal;
        horizontal.canceled += Horizontal;

        // Vertical�Ɋ��蓖�Ă�
        var vertical = _playerInput.Player.Vertical;
        vertical.started += Vertical;
        vertical.canceled += Vertical;

        // Slowdown�Ɋ��蓖�Ă�
        var slowdown = _playerInput.Player.Slowdown;
        slowdown.started += Slowdown;
        slowdown.canceled += Slowdown;
    }

    protected override void OnUpdate()
    {
        var delta = SystemAPI.Time.DeltaTime;
        var elapsed = SystemAPI.Time.ElapsedTime;

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
            var speed = (playerData.ValueRO.isSlowdown) ? playerData.ValueRO.slowMoveSpeed : playerData.ValueRO.moveSpeed;

            // �������͂𐅕������ɉ��Z���
            currentPosition.x += _horizontalValue * speed * delta;

            // �������͂𐂒������ɉ��Z���
            currentPosition.z += _verticalValue * speed * delta;

            // ���f
            localTfm.ValueRW.Position = currentPosition;

            // �J�����ʒu�ɔ��f
            GameManager.Instance.GameCameraPosition = currentPosition;
        }

        if (_isShot)
        {
            // PlayerTag�̕t����Shooter�Ŕ��˂���
            // AutoShooter��Shot�������Ȃ��Ă�����Ɏˌ�����
            // �G�ɂ��Ă��A�G���璼�ڔ��˂���̂ł͂Ȃ�ShooterPrefab���甭�˂���悤�ɐ݌v����



            // �ˌ�����
            foreach (var (playerTag, playerData, localTfm) in
                SystemAPI.Query<
                    RefRO<PlayerTag>,
                    RefRW<PlayerData>,
                    RefRW<LocalTransform>>())
            {
                // �Ō�Ɏˌ����������ƌ��݂̎��Ԃ���o�ߎ��Ԃ����߁A�ˌ��Ԋu�𖞂����Ă��Ȃ���΃R���e�j���[
                if (elapsed - playerData.ValueRW.lastShotTime <= playerData.ValueRO.firingInterval) { continue; }

                // �Ō�Ɏˌ������������X�V
                playerData.ValueRW.lastShotTime = elapsed;

                // PL�̒e�𐶐�
                var playerBulletEntity = EntityManager.Instantiate(playerData.ValueRO.playerBulletEntity);

                // �e��LocalTransform���擾
                var localTransform = SystemAPI.GetComponent<LocalTransform>(playerBulletEntity);

                // ���ݒn����
                localTransform.Position = localTfm.ValueRO.Position;

                // �ύX��K�p
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
    /// �������͂̏���
    /// </summary>
    /// <param name="context"></param>
    private void Horizontal(InputAction.CallbackContext context)
    {
        _horizontalValue = context.ReadValue<float>();
    }

    /// <summary>
    /// �������͂̏���
    /// </summary>
    /// <param name="context"></param>
    private void Vertical(InputAction.CallbackContext context)
    {
        _verticalValue = context.ReadValue<float>();
    }

    /// <summary>
    /// �������͂̏���
    /// </summary>
    /// <param name="context"></param>
    private void Slowdown(InputAction.CallbackContext context)
    {
        Debug.Log("��������PL�͈�̂������Ȃ��̂�����V���O���g���ł����̂ł́H");

        switch (context.phase)
        {
            case InputActionPhase.Started:
                // ��������
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
                // ��������
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
