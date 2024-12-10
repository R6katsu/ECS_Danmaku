using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static MoveHelper;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �{�X�G�ɕK�v�ȏ��
/// </summary>
public struct BossEnemySingletonData : IComponentData
{
    [Tooltip("PlayerTrackingNWay��n-Way�e")]
    public readonly NWay_DanmakuData playerTrackingNWay;

    [Tooltip("RotationNWay��n-Way�e")]
    public readonly NWay_DanmakuData rotationNWay;

    [Tooltip("RandomSpreadBullets��n-Way�e")]
    public readonly NWay_DanmakuData randomSpreadBulletsNWay;

    [Tooltip("RotationNWay��RotationData")]
    public readonly RotationData rotation;

    [Tooltip("�U�����؂�ւ��܂ł̎���")]
    public readonly float attackSwitchTime;

    /// <summary>
    /// �{�X�G�̏��
    /// </summary>
    /// <param name="playerTrackingNWay">PlayerTrackingNWay��n-Way�e�̐ݒ�</param>
    /// <param name="rotationNWay">RotationNWay��n-Way�e�̐ݒ�</param>
    /// <param name="randomSpreadBulletsNWay">RandomSpreadBullets��n-Way�e�̐ݒ�</param>
    /// <param name="rotation">RotationNWay��RotationData</param>
    /// <param name="attackSwitchTime">�U�����؂�ւ��܂ł̎���</param>
    public BossEnemySingletonData
    (
        NWay_DanmakuData playerTrackingNWay,
        NWay_DanmakuData rotationNWay,
        NWay_DanmakuData randomSpreadBulletsNWay,
        RotationData rotation,
        float attackSwitchTime
    )
    {
        // nWay�e
        this.playerTrackingNWay = playerTrackingNWay;
        this.rotationNWay = rotationNWay;
        this.randomSpreadBulletsNWay = randomSpreadBulletsNWay;

        // ��]
        this.rotation = rotation;

        // �U�����؂�ւ��܂ł̎���
        this.attackSwitchTime = attackSwitchTime;
    }
}

/// <summary>
/// �{�X�G�ɕK�v�Ȑݒ�
/// </summary>
public class BossEnemyAuthoring : SingletonMonoBehaviour<BossEnemyAuthoring>
{
    [SerializeField, Header("PlayerTrackingNWay��n-Way�e�̐ݒ�")]
    private NWay_DanmakuSettingParameter _playerTrackingNWay = new();

    [SerializeField, Header("RotationNWay��n-Way�e�̐ݒ�")]
    private NWay_DanmakuSettingParameter _rotationNWay = new();

    [SerializeField, Header("RandomSpreadBullets��n-Way�e�̐ݒ�")]
    private NWay_DanmakuSettingParameter _randomSpreadBulletsNWay = new();

    [SerializeField, Header("���g�̉�]�ɕK�v�Ȑݒ�")]
    private RotationData _rotationData = new();

    [SerializeField, Min(0.0f), Header("�U�����؂�ւ��܂ł̎���")]
    private float _attackSwitchTime = 0.0f;

    public class Baker : Baker<BossEnemyAuthoring>
    {
        public override void Bake(BossEnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Prefab��Entity��
            var playerTrackingNWay = GetEntity(src._playerTrackingNWay.BulletPrefab, TransformUsageFlags.Dynamic);
            var rotationNWay = GetEntity(src._rotationNWay.BulletPrefab, TransformUsageFlags.Dynamic);
            var randomSpreadBulletsNWay = GetEntity(src._randomSpreadBulletsNWay.BulletPrefab, TransformUsageFlags.Dynamic);

            var bossEnemySingletonData = new BossEnemySingletonData
            (
                new NWay_DanmakuData(src._playerTrackingNWay, playerTrackingNWay),
                new NWay_DanmakuData(src._rotationNWay, rotationNWay),
                new NWay_DanmakuData(src._randomSpreadBulletsNWay, randomSpreadBulletsNWay),
                src._rotationData,
                src._attackSwitchTime
            );

            // Data���A�^�b�`
            AddComponent(entity, bossEnemySingletonData);
        }
    }
}
