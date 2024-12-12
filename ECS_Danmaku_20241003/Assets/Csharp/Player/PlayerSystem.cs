using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using static BulletHelper;
using Unity.Physics;
using static EntityCampsHelper;
using static HealthPointDatas;
using Unity.Burst;

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

// ���t�@�N�^�����O�ς�

/// <summary>
/// PL�̏���
/// </summary>
public partial class PlayerSystem : SystemBase
{
    // InputSystem
    private PlayerControls _playerInput = null;

    // ����
    private float _horizontalValue = 0.0f;
    private float _verticalValue = 0.0f;

    private ComponentLookup<PlayerSingletonData> _playerSingletonLookup = new();
    private ComponentLookup<HealthPointData> _healthPointLookup = new();
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup = new();
    private ComponentLookup<DestroyableData> _destroyableLookup = new();
    private ComponentLookup<RemainingPierceCountData> _remainingPierceCountLookup = new();
    private ComponentLookup<LocalTransform> _localTransformLookup = new();
    private ComponentLookup<VFXCreationData> _vfxCreationLookup = new();
    private ComponentLookup<AudioPlayData> _audioPlayLookup = new();

    [Tooltip("���ߍU����")]
    private bool _isChargeShot = false;

    [Tooltip("������")]
    private bool _isSlowdown = false;

    [Tooltip("�Ō�ɍU����������������")]
    private double _lastShotTime = 0.0f;

    [Tooltip("���݂̃`���[�W����")]
    private float _currentChargeTime = 0.0f;

    /// <summary>
    /// ���݂̃`���[�W����
    /// </summary>
    private float CurrentChargeTime
    {
        get => _currentChargeTime;
        set
        {
            _currentChargeTime = value;

            if (_currentChargeTime <= 0)
            {
                // �`���[�W�ő�
                // ����A��������0�ɂȂ���Ċ����ł����̂ł́H
                // �`���[�W���Ԃ�n�ȏゾ�����ꍇ�̓`���[�W�����̂��ߗ������痭�ߍU���I�ł�����
                // �i�K�����ł����炷��
            }
        }
    }

    protected override void OnCreate()
    {
        // �擾����
        _playerSingletonLookup = GetComponentLookup<PlayerSingletonData>(false);
        _healthPointLookup = GetComponentLookup<HealthPointData>(false);
        _dealDamageLookup = GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = GetComponentLookup<DestroyableData>(false);
        _remainingPierceCountLookup = GetComponentLookup<RemainingPierceCountData>(false);
        _localTransformLookup = GetComponentLookup<LocalTransform>(false);
        _vfxCreationLookup = GetComponentLookup<VFXCreationData>(false);
        _audioPlayLookup = GetComponentLookup<AudioPlayData>(false);

        // ShooterControls���C���X�^���X�����A�L���ɂ���
        // ���\�[�X����ׂ̈Ƀt�B�[���h�ϐ��Ƃ��ĕێ�����
        _playerInput = new PlayerControls();
        _playerInput.Enable();

        // ChargeShot�Ɋ��蓖�Ă�
        var chargeShot = _playerInput.Player.ChargeShot;
        chargeShot.started += (context) => _isChargeShot = true;
        chargeShot.canceled += (context) => _isChargeShot = false;

        // Horizontal�Ɋ��蓖�Ă�
        var horizontal = _playerInput.Player.Horizontal;
        horizontal.started += Horizontal;
        horizontal.started += TiltOnMoveStart;
        horizontal.canceled += Horizontal;
        horizontal.canceled += TiltOnMoveEnd;

        // Vertical�Ɋ��蓖�Ă�
        var vertical = _playerInput.Player.Vertical;
        vertical.started += Vertical;
        vertical.canceled += Vertical;

        // Slowdown�Ɋ��蓖�Ă�
        var slowdown = _playerInput.Player.Slowdown;
        slowdown.started += (context) => _isSlowdown = true;
        slowdown.canceled += (context) => _isSlowdown = false;
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

        var delta = SystemAPI.Time.DeltaTime;       // �t���[���b��
        var elapsed = SystemAPI.Time.ElapsedTime;   // �o�ߎ���

        // �ړ�����
        PlayerMove(delta, playerEntity, playerSingleton, playerTransform);

        if (_isChargeShot)
        {
            _currentChargeTime += delta;
            //chargeTime
        }
        else
        {
            // �ˌ�����
            PlayerShot(elapsed, playerEntity, playerSingleton, playerTransform);
        }

        // PL�̔�e���̏���
        PlayerDamageTriggerJob();
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
    /// PL�̈ړ�����
    /// </summary>
    /// <param name="delta">���݂̃t���[���b��</param>
    /// <param name="playerEntity">PL��Entity</param>
    /// <param name="playerSingleton">PL��SingletonData</param>
    /// <param name="playerTransform">PL��LocalTransform</param>
    public void PlayerMove(float delta, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // ���͂��Ȃ�����
        if (_verticalValue == 0.0f && _horizontalValue == 0.0f) { return; }

        // MovementRangeSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // PL�̈ړ��\�͈͂��擾
        var playerMovementRange = movementRangeSingleton.PlayerMovementRange;

        // ���S�ʒu
        var movementRangeCenter = playerMovementRange.movementRangeCenter;

        // �����̑傫�������߂�
        var halfMovementRange = playerMovementRange.movementRange.Halve();

        // ���S�ʒu���l�������ړ��\�͈͂����߂�
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // ���ݒn���擾
        var currentPosition = playerTransform.Position;

        // �ړ����x
        var speed = (_isSlowdown) ? playerSingleton.slowMoveSpeed : playerSingleton.moveSpeed;

        // �������́A�������͂���float3���쐬
        var movementDirection = new float3(_horizontalValue, 0.0f, _verticalValue);

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

        // �ύX�𔽉f
        SystemAPI.SetComponent(playerEntity, playerTransform);
    }

    /// <summary>
    /// PL�̒ʏ�U��
    /// </summary>
    /// <param name="elapsed">�o�ߎ���</param>
    /// <param name="playerEntity">PLEntity</param>
    /// <param name="playerSingleton">PLData</param>
    /// <param name="playerTransform">PLTfm</param>
    public void PlayerShot(double elapsed, Entity playerEntity, PlayerSingletonData playerSingleton, LocalTransform playerTransform)
    {
        // �Ō�Ɏˌ����������ƌ��݂̎��Ԃ���o�ߎ��Ԃ����߁A�ˌ��Ԋu�𖞂����Ă��Ȃ���ΐ؂�グ��
        if (elapsed - _lastShotTime <= playerSingleton.firingInterval) { return; }

        // �Ō�Ɏˌ������������X�V
        _lastShotTime = elapsed;

        // PL�̒e�𐶐�
        var playerBulletEntity = EntityManager.Instantiate(playerSingleton.playerBulletEntity);

        // �e��LocalTransform���擾
        var bulletTfm = SystemAPI.GetComponent<LocalTransform>(playerBulletEntity);

        // ���ݒn����
        bulletTfm.Position = playerTransform.Position;

        // ���݂̉�]����
        bulletTfm.Rotation = playerTransform.Rotation;

        // �ύX��K�p
        SystemAPI.SetComponent(playerBulletEntity, bulletTfm);
        SystemAPI.SetComponent(playerEntity, playerSingleton);
    }

    /// <summary>
    /// �ړ����ɑ̂��X���鏈�����J�n
    /// </summary>
    /// <param name="context"></param>
    private void TiltOnMoveStart(InputAction.CallbackContext context)
    {
        // PlayerSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonData������Entity���擾
        Entity modelEntity = playerSingleton.playerModelEntity;

        // LocalTransform���������Ă��Ȃ�����
        if (!SystemAPI.HasComponent<LocalTransform>(modelEntity)) { return; }

        // Model����LocalTransform���擾
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(modelEntity);

        // 1�A0�A-1�̐����ɕϊ�
        var dir = (int)math.sign(_horizontalValue);

        // �ύX��K�p
        playerTransform.Rotation = quaternion.EulerXYZ(0, math.radians(playerSingleton.playerMoveTilt * dir), 0);
        SystemAPI.SetComponent(modelEntity, playerTransform);
    }

    /// <summary>
    /// �ړ����ɑ̂��X���鏈�����I��
    /// </summary>
    /// <param name="context"></param>
    public void TiltOnMoveEnd(InputAction.CallbackContext context)
    {
        // PlayerSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // PlayerSingletonData������Entity���擾
        Entity modelEntity = playerSingleton.playerModelEntity;

        // LocalTransform���������Ă��Ȃ�����
        if (!SystemAPI.HasComponent<LocalTransform>(modelEntity)) { return; }

        // Model����LocalTransform���擾
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(modelEntity);

        // �X����������
        playerTransform.Rotation = Quaternion.identity;

        // �ύX��K�p
        SystemAPI.SetComponent(modelEntity, playerTransform);
    }

    /// <summary>
    /// PL�̔�e���̏������Ăяo��
    /// </summary>
    private void PlayerDamageTriggerJob()
    {
        // �X�V����
        _playerSingletonLookup.Update(this);
        _healthPointLookup.Update(this);
        _dealDamageLookup.Update(this);
        _destroyableLookup.Update(this);
        _remainingPierceCountLookup.Update(this);
        _localTransformLookup.Update(this);
        _vfxCreationLookup.Update(this);
        _audioPlayLookup.Update(this);

        // PL�ɒe�������������̏������Ăяo��
        var playerDamage = new PlayerDamageTriggerJob()
        {
            playerSingletonLookup = _playerSingletonLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            localTransformLookup = _localTransformLookup,
            vfxCreationLookup = _vfxCreationLookup,
            audioPlayLookup = _audioPlayLookup
        };

        // �O�̃W���u����������
        Dependency.Complete();

        var playerJobHandle = playerDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        Dependency = playerJobHandle;
    }
}

/// <summary>
/// �ڐG���Ƀ_���[�W���󂯂�
/// </summary>
public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
{
    // �C���X�^���X�̎擾�ɕK�v�ȕϐ�
    public ComponentLookup<PlayerSingletonData> playerSingletonLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;
    public ComponentLookup<LocalTransform> localTransformLookup;
    public ComponentLookup<VFXCreationData> vfxCreationLookup;
    public ComponentLookup<AudioPlayData> audioPlayLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
        var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

        // entityB��LocalTransform��L���Ă��邩
        if (!localTransformLookup.HasComponent(entityB)) { return; }

        // isTrigger���L���ł���ȏ�ALocalTransform�͐�΂ɂ���
        var localTransform = localTransformLookup[entityB];
        var position = localTransform.Position;

        // entityA��BulletIDealDamageData��L���Ă��Ȃ��B
        // ���邢�́AentityB��PlayerSingletonData��L���Ă��Ȃ���ΐ؂�グ��
        if (!dealDamageLookup.HasComponent(entityA) || !playerSingletonLookup.HasComponent(entityB)) { return; }

        // entityB����PlayerHealthPointData���擾
        var playerSingleton = playerSingletonLookup[entityB];

        // entityA����BulletIDealDamageData���擾
        var dealDamage = dealDamageLookup[entityA];

        // �_���[�W���̐w�c�̎�ނ�Player��������؂�グ��
        if (dealDamage.campsType == EntityCampsType.Player) { return; }

        // �U�����󂯂��Ώۂ�DestroyableData��L���Ă���
        if (destroyableLookup.HasComponent(entityB))
        {
            var destroyable = destroyableLookup[entityB];

            // �폜�t���O��������
            destroyable.isKilled = true;

            // �ύX�𔽉f
            destroyableLookup[entityB] = destroyable;

            // �폜�t���O��������
            // �����Entity���폜����鎞�̏����Ƃ��Ēǉ��ł��Ȃ���
            if (destroyable.isKilled)
            {
                // �Q�[���I�[�o�[�������J�n����
                GameManager.Instance.MyGameState = GameState.GameOver;

                // EntityB��VFXCreationData��L���Ă���
                if (vfxCreationLookup.HasComponent(entityB))
                {
                    var vfxCreation = vfxCreationLookup[entityB];
                    vfxCreation.Position = position;
                    vfxCreationLookup[entityB] = vfxCreation;
                }

                // EntityB��AudioPlayData��L���Ă���
                if (audioPlayLookup.HasComponent(entityB))
                {
                    var audioPlay = audioPlayLookup[entityB];
                    audioPlay.AudioNumber = playerSingleton.killedSENumber;
                    audioPlayLookup[entityB] = audioPlay;
                }
            }
        }
    }
}