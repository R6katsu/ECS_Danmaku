using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Unity.Collections;


#if UNITY_EDITOR
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
#endif

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

    [Tooltip("�{�X�G�̏��")]
    private BossEnemyState _bossEnemyState = BossEnemyState.None;

    [Tooltip("�U�����؂�ւ��܂ł̎���")]
    private float _attackSwitchTime = 10.0f;     // �C���X�y�N�^����ݒ�ł���悤�ɂ���

    [Tooltip("���݂̍U�����؂�ւ��܂ł̎���")]
    private float _currentAttackSwitchTime = 0.0f;

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
                // �U�����؂�ւ��܂ł̎��Ԃ��ő�l�։�
                _currentAttackSwitchTime = _attackSwitchTime;

                // BossEnemyState�ɑΉ����郊�Z�b�g����
                ResetBossEnemyState(_bossEnemyState);

                // ���f
                _bossEnemyState = value;

                // BossEnemyState�ɑΉ����鏈��
                ChangeBossEnemyState(_bossEnemyState);
            }
        }
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

        // 0�ȉ��ɂȂ����玟�̍U���֐؂�ւ���
        _currentAttackSwitchTime -= SystemAPI.Time.DeltaTime;
        if (_currentAttackSwitchTime <= 0.0f)
        {
            MyBossEnemyState = GetRandomBossEnemyState();
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
                MyBossEnemyState = GetRandomBossEnemyState();
                break;
        }
    }

    /// <summary>
    /// ���݂�BossEnemyState�ɑΉ����郊�Z�b�g����
    /// </summary>
    private void ResetBossEnemyState(BossEnemyState bossEnemyState)
    {
        // nWay�Ƃ΂�T�������ǂ���



        // BossEnemySingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<BossEnemySingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var bossEnemySingleton = SystemAPI.GetSingleton<BossEnemySingletonData>();

        // Entity���擾
        Entity entity = SystemAPI.GetSingletonEntity<BossEnemySingletonData>();

        switch (bossEnemyState)
        {
            case BossEnemyState.PlayerTrackingNWay:
                // N_Way_DanmakuData��L���Ă���
                if (SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // N_Way_DanmakuData���擾
                    var nWay_DanmakuData = SystemAPI.GetComponent<N_Way_DanmakuData>(entity);

                    nWay_DanmakuData.IsDataDeletion = true;
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationData��L���Ă���
                if (SystemAPI.HasComponent<RotationData>(entity))
                {
                    // RotationData���擾
                    var rotationData = SystemAPI.GetComponent<RotationData>(entity);

                    rotationData.IsDataDeletion = true;
                }

                // N_Way_DanmakuData��L���Ă���
                if (SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // N_Way_DanmakuData���擾
                    var nWay_DanmakuData = SystemAPI.GetComponent<N_Way_DanmakuData>(entity);

                    nWay_DanmakuData.IsDataDeletion = true;
                }
                break;

            case BossEnemyState.RandomSpreadBullets:
                // TapShooting_DanmakuData��L���Ă���
                if (SystemAPI.HasComponent<TapShooting_DanmakuData>(entity))
                {
                    // TapShooting_DanmakuData���擾
                    var tapShooting_DanmakuData = SystemAPI.GetComponent<TapShooting_DanmakuData>(entity);

                    tapShooting_DanmakuData.IsDataDeletion = true;
                }
                break;

            case BossEnemyState.None:
            default:
                break;
        }
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
                // N_Way_DanmakuData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<N_Way_DanmakuData>(entity);

                    Debug.LogError("�}�W�b�N�i���o�[�B�C���X�y�N�^����ݒ肷��ۂ̕��@���l����");

                    var nWay_DanmakuData = new N_Way_DanmakuData
                    (
                        120,
                        12,
                        1f,
                        bossEnemySingleton.nWayBulletEntity,
                        bossEnemySingleton.bulletLocalScale
                    );

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData<N_Way_DanmakuData>(entity, nWay_DanmakuData);
                }
                break;

            case BossEnemyState.RotationNWay:
                // RotationData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<RotationData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<RotationData>(entity);

                    var rotationData = new RotationData()
                    {
                        axisType = AxisType.Y,
                        rotationSpeed = bossEnemySingleton.rotationSpeed
                    };

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData<RotationData>(entity, rotationData);
                }

                // N_Way_DanmakuData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<N_Way_DanmakuData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<N_Way_DanmakuData>(entity);

                    var nWay_DanmakuData = new N_Way_DanmakuData
                    (
                        bossEnemySingleton.fanAngle,
                        bossEnemySingleton.amountBullets,
                        bossEnemySingleton.nWayFiringInterval,
                        bossEnemySingleton.nWayBulletEntity,
                        bossEnemySingleton.bulletLocalScale
                    );

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData<N_Way_DanmakuData>(entity, nWay_DanmakuData);
                }

                break;

            case BossEnemyState.RandomSpreadBullets:
                // TapShooting_DanmakuData��L���Ă��Ȃ�����
                if (!SystemAPI.HasComponent<TapShooting_DanmakuData>(entity))
                {
                    // �A�^�b�`
                    EntityManager.AddComponent<TapShooting_DanmakuData>(entity);

                    var tapShooting_DanmakuData = new TapShooting_DanmakuData
                    (
                        bossEnemySingleton.shootNSingleSet,
                        bossEnemySingleton.singleSetRestTimeAfter,
                        bossEnemySingleton.tapFiringInterval,
                        bossEnemySingleton.tapBulletEntity
                    );

                    // �ϐ���ݒ�
                    EntityManager.SetComponentData<TapShooting_DanmakuData>(entity, tapShooting_DanmakuData);
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
        Array allStates = Enum.GetValues(typeof(BossEnemyState));

        // ������Ԃ����O
        BossEnemyState[] validStates = Array.FindAll(
            allStates as BossEnemyState[],
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
        // �Ȃ񂩕�����Ȃ��B�ȒP�ɔ�������
        // �{�X�G���̂��c�����s�Ɉړ����Ȃ���΂�T�����狥���Ȃ̂ł́H

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
        float theta = Random.Range(0f, Mathf.PI * 2);   // 0�x����360�x�܂ł̃����_���Ȋp�x
        float x = Mathf.Cos(theta);                     // Cos�l���v�Z�i�]���j
        float y = 0.0f;                                 // �������`
        float z = Mathf.Sin(theta);                     // Sin�l���v�Z�i�����j
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
