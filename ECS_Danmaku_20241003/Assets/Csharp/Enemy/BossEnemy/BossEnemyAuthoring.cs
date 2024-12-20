using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static MoveHelper;
#endif

// リファクタリング済み

/// <summary>
/// ボス敵に必要な情報
/// </summary>
public struct BossEnemySingletonData : IComponentData
{
    [Tooltip("PlayerTrackingNWayのn-Way弾")]
    public readonly NWay_DanmakuData playerTrackingNWay;

    [Tooltip("RotationNWayのn-Way弾")]
    public readonly NWay_DanmakuData rotationNWay;

    [Tooltip("RandomSpreadBulletsのn-Way弾")]
    public readonly NWay_DanmakuData randomSpreadBulletsNWay;

    [Tooltip("RotationNWayのRotationData")]
    public readonly RotationData rotation;

    [Tooltip("攻撃が切り替わるまでの時間")]
    public readonly float attackSwitchTime;

    /// <summary>
    /// ボス敵の情報
    /// </summary>
    /// <param name="playerTrackingNWay">PlayerTrackingNWayのn-Way弾の設定</param>
    /// <param name="rotationNWay">RotationNWayのn-Way弾の設定</param>
    /// <param name="randomSpreadBulletsNWay">RandomSpreadBulletsのn-Way弾の設定</param>
    /// <param name="rotation">RotationNWayのRotationData</param>
    /// <param name="attackSwitchTime">攻撃が切り替わるまでの時間</param>
    public BossEnemySingletonData
    (
        NWay_DanmakuData playerTrackingNWay,
        NWay_DanmakuData rotationNWay,
        NWay_DanmakuData randomSpreadBulletsNWay,
        RotationData rotation,
        float attackSwitchTime
    )
    {
        // nWay弾
        this.playerTrackingNWay = playerTrackingNWay;
        this.rotationNWay = rotationNWay;
        this.randomSpreadBulletsNWay = randomSpreadBulletsNWay;

        // 回転
        this.rotation = rotation;

        // 攻撃が切り替わるまでの時間
        this.attackSwitchTime = attackSwitchTime;
    }
}

/// <summary>
/// ボス敵に必要な設定
/// </summary>
public class BossEnemyAuthoring : SingletonMonoBehaviour<BossEnemyAuthoring>
{
    [SerializeField, Header("PlayerTrackingNWayのn-Way弾の設定")]
    private NWay_DanmakuSettingParameter _playerTrackingNWay = new();

    [SerializeField, Header("RotationNWayのn-Way弾の設定")]
    private NWay_DanmakuSettingParameter _rotationNWay = new();

    [SerializeField, Header("RandomSpreadBulletsのn-Way弾の設定")]
    private NWay_DanmakuSettingParameter _randomSpreadBulletsNWay = new();

    [SerializeField, Header("自身の回転に必要な設定")]
    private RotationData _rotationData = new();

    [SerializeField, Min(0.0f), Header("攻撃が切り替わるまでの時間")]
    private float _attackSwitchTime = 0.0f;

    public class Baker : Baker<BossEnemyAuthoring>
    {
        public override void Bake(BossEnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // PrefabのEntity化
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

            // Dataをアタッチ
            AddComponent(entity, bossEnemySingletonData);
        }
    }
}
