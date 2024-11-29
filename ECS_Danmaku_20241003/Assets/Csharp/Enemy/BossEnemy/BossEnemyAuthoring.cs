using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

/// <summary>
/// ボス敵の情報
/// </summary>
public struct BossEnemySingletonData : IComponentData
{
    [Tooltip("回転速度")]
    public readonly float rotationSpeed;

    [Tooltip("扇の大きさ")]
    public readonly int fanAngle;

    [Tooltip("弾の量")]
    public readonly int amountBullets;

    [Tooltip("nWayの発射間隔")]
    public readonly float nWayFiringInterval;

    [Tooltip("nWayの弾のEntity")]
    public readonly Entity nWayBulletEntity;

    [Tooltip("弾のPrefabの大きさ")]
    public readonly float bulletLocalScale;

    [Tooltip("経過時間")]
    public float elapsedTime;

    [Tooltip("ワンセットでn回撃つ")]
    public readonly int shootNSingleSet;

    [Tooltip("ワンセット終了後の休息時間")]
    public readonly float singleSetRestTimeAfter;

    [Tooltip("タップ撃ちの発射間隔")]
    public readonly float tapFiringInterval;

    [Tooltip("タップ撃ちの弾のPrefabEntity")]
    public readonly Entity tapBulletEntity;

    [Tooltip("現在の射撃回数")]
    public int currentShotCount;

    [Tooltip("次回のワンセット開始時刻")]
    public double singleSetNextTime;

    [Tooltip("次回の射撃時刻")]
    public double firingNextTime;

    /// <summary>
    /// ボス敵の情報
    /// </summary>
    /// <param name="rotationSpeed">回転速度</param>
    public BossEnemySingletonData(float rotationSpeed, int fanAngle, int amountBullets, float nWayFiringInterval, Entity bulletPrefab, float bulletLocalScale, int shootNSingleSet, float singleSetRestTimeAfter, float tapFiringInterval, Entity bulletEntity)
    {
        // 回転
        this.rotationSpeed = rotationSpeed;

        // nWay弾
        this.fanAngle = fanAngle;
        this.amountBullets = amountBullets;
        this.nWayFiringInterval = nWayFiringInterval;
        this.nWayBulletEntity = bulletPrefab;
        this.bulletLocalScale = bulletLocalScale;
        elapsedTime = 0.0f;

        // タップ撃ち
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
/// ボス敵の設定
/// </summary>
public class BossEnemyAuthoring : SingletonMonoBehaviour<BossEnemyAuthoring>
{
    [SerializeField, Header("回転速度")]
    private float _rotationSpeed;

    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("扇の大きさ")]
    private int _fanAngle = 0;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("弾の量")]
    private int _amountBullets = 0;

    [SerializeField, Min(0.0f), Header("n-Wayの発射間隔")]
    private float _nWayFiringInterval = 0.0f;

    [SerializeField, Header("n-Wayの弾のPrefab")]
    private Transform _nWayBulletPrefab = null;

    [SerializeField, Min(0), Header("ワンセットでn回撃つ")]
    private int _shootNSingleSet = 0;

    [SerializeField, Min(0.0f), Header("ワンセット終了後の休息時間")]
    private float _singleSetRestTimeAfter = 0.0f;

    [SerializeField, Min(0.0f), Header("タップ撃ちの発射間隔")]
    private float _tapFiringInterval = 0.0f;

    [SerializeField, Header("タップ撃ちの弾のPrefab")]
    private Transform _tapBulletPrefab = null;

    public class Baker : Baker<BossEnemyAuthoring>
    {
        public override void Bake(BossEnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var nWayBulletEntity = GetEntity(src._nWayBulletPrefab, TransformUsageFlags.Dynamic);
            var tapBulletPrefab = GetEntity(src._tapBulletPrefab, TransformUsageFlags.Dynamic);

            // LocalTransformのScaleがfloatである為、一番大きい軸を求めることにした
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

            // Dataをアタッチ
            AddComponent(entity, bossEnemySingletonData);
        }
    }
}
