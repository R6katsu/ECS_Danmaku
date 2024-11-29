using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

/// <summary>
/// �{�X�G�̏��
/// </summary>
public struct BossEnemySingletonData : IComponentData
{
    [Tooltip("��]���x")]
    public readonly float rotationSpeed;

    [Tooltip("��̑傫��")]
    public readonly int fanAngle;

    [Tooltip("�e�̗�")]
    public readonly int amountBullets;

    [Tooltip("nWay�̔��ˊԊu")]
    public readonly float nWayFiringInterval;

    [Tooltip("nWay�̒e��Entity")]
    public readonly Entity nWayBulletEntity;

    [Tooltip("�e��Prefab�̑傫��")]
    public readonly float bulletLocalScale;

    [Tooltip("�o�ߎ���")]
    public float elapsedTime;

    [Tooltip("�����Z�b�g��n�񌂂�")]
    public readonly int shootNSingleSet;

    [Tooltip("�����Z�b�g�I����̋x������")]
    public readonly float singleSetRestTimeAfter;

    [Tooltip("�^�b�v�����̔��ˊԊu")]
    public readonly float tapFiringInterval;

    [Tooltip("�^�b�v�����̒e��PrefabEntity")]
    public readonly Entity tapBulletEntity;

    [Tooltip("���݂̎ˌ���")]
    public int currentShotCount;

    [Tooltip("����̃����Z�b�g�J�n����")]
    public double singleSetNextTime;

    [Tooltip("����̎ˌ�����")]
    public double firingNextTime;

    /// <summary>
    /// �{�X�G�̏��
    /// </summary>
    /// <param name="rotationSpeed">��]���x</param>
    public BossEnemySingletonData(float rotationSpeed, int fanAngle, int amountBullets, float nWayFiringInterval, Entity bulletPrefab, float bulletLocalScale, int shootNSingleSet, float singleSetRestTimeAfter, float tapFiringInterval, Entity bulletEntity)
    {
        // ��]
        this.rotationSpeed = rotationSpeed;

        // nWay�e
        this.fanAngle = fanAngle;
        this.amountBullets = amountBullets;
        this.nWayFiringInterval = nWayFiringInterval;
        this.nWayBulletEntity = bulletPrefab;
        this.bulletLocalScale = bulletLocalScale;
        elapsedTime = 0.0f;

        // �^�b�v����
        this.shootNSingleSet = shootNSingleSet;
        this.singleSetRestTimeAfter = singleSetRestTimeAfter;
        this.tapFiringInterval = tapFiringInterval;
        this.tapBulletEntity = bulletEntity;
        currentShotCount = 0;
        singleSetNextTime = 0.0f;
        firingNextTime = 0.0f;
    }
}

/// <summary>
/// �{�X�G�̐ݒ�
/// </summary>
public class BossEnemyAuthoring : SingletonMonoBehaviour<BossEnemyAuthoring>
{
    [SerializeField, Header("��]���x")]
    private float _rotationSpeed;

    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("��̑傫��")]
    private int _fanAngle = 0;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("�e�̗�")]
    private int _amountBullets = 0;

    [SerializeField, Min(0.0f), Header("n-Way�̔��ˊԊu")]
    private float _nWayFiringInterval = 0.0f;

    [SerializeField, Header("n-Way�̒e��Prefab")]
    private Transform _nWayBulletPrefab = null;

    [SerializeField, Min(0), Header("�����Z�b�g��n�񌂂�")]
    private int _shootNSingleSet = 0;

    [SerializeField, Min(0.0f), Header("�����Z�b�g�I����̋x������")]
    private float _singleSetRestTimeAfter = 0.0f;

    [SerializeField, Min(0.0f), Header("�^�b�v�����̔��ˊԊu")]
    private float _tapFiringInterval = 0.0f;

    [SerializeField, Header("�^�b�v�����̒e��Prefab")]
    private Transform _tapBulletPrefab = null;

    public class Baker : Baker<BossEnemyAuthoring>
    {
        public override void Bake(BossEnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var nWayBulletEntity = GetEntity(src._nWayBulletPrefab, TransformUsageFlags.Dynamic);
            var tapBulletPrefab = GetEntity(src._tapBulletPrefab, TransformUsageFlags.Dynamic);

            // LocalTransform��Scale��float�ł���ׁA��ԑ傫���������߂邱�Ƃɂ���
            var localScale = src._nWayBulletPrefab.localScale;
            var localScaleMax = Mathf.Max(localScale.x, localScale.y, localScale.z);

            var bossEnemySingletonData = new BossEnemySingletonData
                (
                    src._rotationSpeed,
                    src._fanAngle,
                    src._amountBullets,
                    src._nWayFiringInterval,
                    nWayBulletEntity,
                    localScaleMax,
                    src._shootNSingleSet,
                    src._singleSetRestTimeAfter,
                    src._tapFiringInterval,
                    tapBulletPrefab
                );

            // Data���A�^�b�`
            AddComponent(entity, bossEnemySingletonData);
        }
    }
}
