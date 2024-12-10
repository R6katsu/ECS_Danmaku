using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

#if UNITY_EDITOR
using Unity.Collections;
using static HealthPointDatas;
using static BulletHelper;
using static EntityCampsHelper;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �{�X�G�̏���
/// </summary>
public partial class BossEnemySystem : SystemBase
{
    /// <summary>
    /// �{�X�G�̏��
    /// </summary>
    private enum BossEnemyState : byte
    {
        [Tooltip("�������")] None,
        [Tooltip("PL�_����n-Way�e")] PlayerTrackingNWay,
        [Tooltip("��]n-Way�e")] RotationNWay,
        [Tooltip("�����_���ɒe���΂�T��")] RandomSpreadBullets
    }

    [Tooltip("�~���̃��W�A���l")]
    private const float FULL_CIRCLE_RADIANS = Mathf.PI * 2;

    [Tooltip("�{�X�G�̏��")]
    private BossEnemyState _bossEnemyState = BossEnemyState.None;

    [Tooltip("�O��̃{�X�G�̏��")]
    private BossEnemyState _lastBossEnemyState = BossEnemyState.None;

    [Tooltip("���݂̍U�����؂�ւ��܂ł̎���")]
    private float _currentAttackSwitchTime = 0.0f;

    private EntityCommandBufferSystem _ecbSystem = null;

    /// <summary>
    /// �{�X�G�̏��
    /// </summary>
    private BossEnemyState MyBossEnemyState
    {
        get => _bossEnemyState;
        set
        {
            // �O�񂩂�ύX��������
            if (_bossEnemyState != value)
            {
                // �O���BossEnemyState�Ƃ��ĕێ�
                _lastBossEnemyState = MyBossEnemyState;

                // ���f
                _bossEnemyState = value;

                // BossEnemyState�ɑΉ����鏈��
                ChangeBossEnemyState(_bossEnemyState);
            }
        }
    }

    protected override void OnCreate()
    {
        // EndSimulationEntityCommandBufferSystem�ōŌ�ɔ��f
        _ecbSystem = World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // BossEnemySingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>())
        {
            // ������
            Initialize();
            return;
        }

        // �V���O���g���f�[�^�̎擾
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entity���擾
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        // �ړ���������
        if (SystemAPI.HasComponent<MoveToTargetPointData>(entity)) { return; }

        // 0�ȉ��ɂȂ����玟�̍U���֐؂�ւ���
        _currentAttackSwitchTime -= SystemAPI.Time.DeltaTime;
        if (_currentAttackSwitchTime <= 0.0f)
        {
            // �U�����؂�ւ��܂ł̎��Ԃ��ő�l�։�
            _currentAttackSwitchTime = bossEnemySingleton.attackSwitchTime;

            // BossEnemyState�ɑΉ����郊�Z�b�g����
            ResetBossEnemyState(_bossEnemyState);

            // 1�t���[���ҋ@�ׂ̈�None���o�R
            MyBossEnemyState = BossEnemyState.None;
            return;
        }

        switch (MyBossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // PL�_����n-Way�e
                PlayerTrackingNWay();
                break;

            case BossEnemyState.RotationNWay:
                // ��]n-Way�e
                RotationNWay();
                break;

            case BossEnemyState.RandomSpreadBullets:
                // �����_���ɒe���΂�T��
                RandomSpreadBullets();
                break;

            case BossEnemyState.None:
            default:
                // �O��̍U���Ɠ�����������ύX���Ȃ�
                var bossEnemyState = GetRandomBossEnemyState();
                MyBossEnemyState = (_lastBossEnemyState == bossEnemyState) ? MyBossEnemyState : bossEnemyState;
                break;
        }
    }

    /// <summary>
    /// ���݂�BossEnemyState�ɑΉ����郊�Z�b�g����
    /// </summary>
    private void ResetBossEnemyState(BossEnemyState bossEnemyState)
    {
        // BossEnemySingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entity���擾
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();

        switch (bossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // N_Way_DanmakuData��L���Ă���
                if (SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    ecb.RemoveComponent<NWay_DanmakuData>(entity);
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationData��L���Ă���
                if (SystemAPI.HasComponent<RotationData>(entity))
                {
                    ecb.RemoveComponent<RotationData>(entity);
                }

                // N_Way_DanmakuData��L���Ă���
                if (SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    ecb.RemoveComponent<NWay_DanmakuData>(entity);
                }
                break;

            case BossEnemyState.RandomSpreadBullets:
                // N_Way_DanmakuData��L���Ă���
                if (SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    ecb.RemoveComponent<NWay_DanmakuData>(entity);
                }
                break;

            case BossEnemyState.None:
            default:
                break;
        }

        // �t���[���I�����ɓK�p
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }

    /// <summary>
    /// BossEnemyState�ɑΉ����鏈��
    /// </summary>
    private void ChangeBossEnemyState(BossEnemyState bossEnemyState)
    {
        // BossEnemySingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entity���擾
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        switch (bossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // NWay_DanmakuData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<NWay_DanmakuData>(entity);

                    var nWay_DanmakuData = new NWay_DanmakuData
                    (
                        bossEnemySingleton.playerTrackingNWay.fanAngle,
                        bossEnemySingleton.playerTrackingNWay.amountBullets,
                        bossEnemySingleton.playerTrackingNWay.firingInterval,
                        bossEnemySingleton.playerTrackingNWay.bulletEntity
                    );

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData(entity, nWay_DanmakuData);
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<RotationData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<RotationData>(entity);

                    var rotationData = new RotationData
                    (
                        bossEnemySingleton.rotation.AxisType,
                        bossEnemySingleton.rotation.RotationSpeed
                    );

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData(entity, rotationData);
                }

                // NWay_DanmakuData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<NWay_DanmakuData>(entity);

                    var nWay_DanmakuData = new NWay_DanmakuData
                    (
                        bossEnemySingleton.rotationNWay.fanAngle,
                        bossEnemySingleton.rotationNWay.amountBullets,
                        bossEnemySingleton.rotationNWay.firingInterval,
                        bossEnemySingleton.rotationNWay.bulletEntity
                    );

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData(entity, nWay_DanmakuData);
                }

                break;

            case BossEnemyState.RandomSpreadBullets:
                // NWay_DanmakuData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<NWay_DanmakuData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<NWay_DanmakuData>(entity);

                    var nWay_DanmakuData = new NWay_DanmakuData
                    (
                        bossEnemySingleton.randomSpreadBulletsNWay.fanAngle,
                        bossEnemySingleton.randomSpreadBulletsNWay.amountBullets,
                        bossEnemySingleton.randomSpreadBulletsNWay.firingInterval,
                        bossEnemySingleton.randomSpreadBulletsNWay.bulletEntity
                    );

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData(entity, nWay_DanmakuData);
                }
                break;

            case BossEnemyState.None:
            default:
                break;
        }
    }

    /// <summary>
    /// �����_����BossEnemyState�𒊑I
    /// </summary>
    /// <returns>���I����BossEnemyState</returns>
    private BossEnemyState GetRandomBossEnemyState()
    {
        // BossEnemyState�̗v�f��S�Ď擾
        Array bossEnemyStateArray = Enum.GetValues(typeof(BossEnemyState));

        // ������Ԃ����O
        BossEnemyState[] validStates = Array.FindAll
        (
            bossEnemyStateArray as BossEnemyState[],
            state => state != BossEnemyState.None
        );

        // ���I���ĕԂ�
        return validStates[Random.Range(0, validStates.Length)];
    }

    /// <summary>
    /// PL�_����n-Way�e
    /// </summary>
    private void PlayerTrackingNWay()
    {
        // BossEnemySingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entity���擾
        Entity bossEntity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        // PlayerSingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<PlayerSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var playerSingleton = SystemAPI.GetSingleton<PlayerSingletonData>();

        // Entity���擾
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerSingletonData>();

        // LocalTransform���������Ă��Ȃ�����
        if (!SystemAPI.HasComponent<LocalTransform>(playerEntity)) { return; }

        // LocalTransform���擾
        var playerTfm = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // LocalTransform���������Ă��Ȃ�����
        if (!SystemAPI.HasComponent<LocalTransform>(bossEntity)) { return; }

        // LocalTransform���擾
        var bossTfm = SystemAPI.GetComponent<LocalTransform>(bossEntity);

        // ��]��K�p
        Quaternion targetRotation = Quaternion.LookRotation(playerTfm.Position - bossTfm.Position);
        bossTfm.Rotation = targetRotation;
        EntityManager.SetComponentData(bossEntity, bossTfm);
    }

    /// <summary>
    /// ��]���Ȃ���n-Way�e
    /// </summary>
    private void RotationNWay()
    {
#if UNITY_EDITOR
        Debug.Log("��]���Ȃ���n-Way�e");
#endif
    }

    /// <summary>
    /// �����_���ɒe���΂�T��
    /// </summary>
    private void RandomSpreadBullets()
    {
        // BossEnemySingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entity���擾
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        // LocalTransform���������Ă��Ȃ�����
        if (!SystemAPI.HasComponent<LocalTransform>(entity)) { return; }

        // LocalTransform���擾
        var localTfm = SystemAPI.GetComponent<LocalTransform>(entity);

        // �~���̃����_���Ȉʒu���v�Z
        float theta = Random.Range(0f, FULL_CIRCLE_RADIANS);    // 0�x����360�x�܂ł̃����_���Ȋp�x
        float x = Mathf.Cos(theta);                             // Cos�l���v�Z�i�]���j
        float y = 0.0f;                                         // �������`
        float z = Mathf.Sin(theta);                             // Sin�l���v�Z�i�����j
        Vector3 randomDirection = new Vector3(x, y, z);

        // ��]��K�p
        Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
        localTfm.Rotation = targetRotation;
        EntityManager.SetComponentData(entity, localTfm);
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Initialize()
    {
        MyBossEnemyState = BossEnemyState.None;
    }
}
