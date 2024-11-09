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
        // PlayerSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonData������Entity���擾
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // LocalTransform���������Ă��Ȃ�����
        if (!SystemAPI.HasComponent<LocalTransform>(playerEntity)) { return; }

        // PL����LocalTransform���擾
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // �t���[���b���A�o�ߎ���
        var delta = SystemAPI.Time.DeltaTime;
        var elapsed = SystemAPI.Time.ElapsedTime;

        // �ړ�����
        PlayerMove(delta, playerEntity, playerSingleton, playerTransform);

        if (_isShot)
        {
            // PlayerTag�̕t����Shooter�Ŕ��˂���
            // AutoShooter��Shot�������Ȃ��Ă�����Ɏˌ�����
            // �G�ɂ��Ă��A�G���璼�ڔ��˂���̂ł͂Ȃ�ShooterPrefab���甭�˂���悤�ɐ݌v����



            // �ˌ�����
            PlayerShot(elapsed, playerEntity, playerSingleton, playerTransform);
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
        // PlayerSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        switch (context.phase)
        {
            case InputActionPhase.Started:
                // �����t���O�𗧂Ă�
                playerSingleton.isSlowdown = true;
                break;

            case InputActionPhase.Canceled:
                // �����t���O��܂�
                playerSingleton.isSlowdown = false;
                break;
        }

        // PlayerSingletonData������Entity���擾
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // �ύX�𔽉f
        SystemAPI.SetComponent(playerEntity, playerSingleton);
    }

    /// <summary>
    /// PL�̈ړ�����
    /// </summary>
    /// <param name="delta">���݂̃t���[���b��</param>
    /// <param name="playerEntity">PL��Entity</param>
    /// <param name="playerSingleton">PL��SingletonData</param>
    /// <param name="playerTransform">PL��LocalTransform</param>
    public void PlayerMove(float delta, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // MovementRangeSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // PL�̈ړ��\�͈͂��擾
        var playerMovementRange = movementRangeSingleton.playerMovementRange;

        // ���S�ʒu
        var movementRangeCenter = playerMovementRange.movementRangeCenter;

        // �����̑傫�������߂�
        var halfMovementRange = playerMovementRange.movementRange / 2;

        // ���S�ʒu���l�������ړ��\�͈͂����߂�
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // ���ݒn���擾
        var currentPosition = playerTransform.Position;

        // �ړ����x
        var speed = (playerSingleton.isSlowdown) ? playerSingleton.slowMoveSpeed : playerSingleton.moveSpeed;

        // �������́A�������͂���float3���쐬
        var movementDirection = new float3(_horizontalValue, new float(), _verticalValue);

        // ���͂�����ꍇ�͐��K��
        movementDirection = (math.lengthsq(movementDirection) > 0) ?
                math.normalize(movementDirection) : movementDirection;

        // �ړ��ʂ��Z�o
        Vector3 scaledMovement = movementDirection * speed * delta;

        // �ړ������Z���
        currentPosition += (float3)scaledMovement;

        // �ړ��\�͈͓��Ɏ��߂�
        currentPosition.x = Mathf.Clamp(currentPosition.x, minMovementRange.x, maxMovementRange.x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minMovementRange.y, maxMovementRange.y);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minMovementRange.z, maxMovementRange.z);

        // ���f
        playerTransform.Position = currentPosition;

        // �J�����ʒu�ɔ��f
        GameManager.Instance.GameCameraPosition = currentPosition;

        // �ύX�𔽉f
        SystemAPI.SetComponent(playerEntity, playerTransform);
    }

    public void PlayerShot(double elapsed, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // �Ō�Ɏˌ����������ƌ��݂̎��Ԃ���o�ߎ��Ԃ����߁A�ˌ��Ԋu�𖞂����Ă��Ȃ���ΐ؂�グ��
        if (elapsed - playerSingleton.lastShotTime <= playerSingleton.firingInterval) { return; }

        // �Ō�Ɏˌ������������X�V
        playerSingleton.lastShotTime = elapsed;

        // PL�̒e�𐶐�
        var playerBulletEntity = EntityManager.Instantiate(playerSingleton.playerBulletEntity);

        // �e��LocalTransform���擾
        var localTransform = SystemAPI.GetComponent<LocalTransform>(playerBulletEntity);

        // ���ݒn����
        localTransform.Position = playerTransform.Position;

        // �ύX��K�p
        SystemAPI.SetComponent(playerBulletEntity, localTransform);
        SystemAPI.SetComponent(playerEntity, playerSingleton);
    }
}
