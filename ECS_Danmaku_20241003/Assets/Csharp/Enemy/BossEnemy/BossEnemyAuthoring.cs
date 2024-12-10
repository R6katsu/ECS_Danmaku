using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static MoveHelper;
#endif

// ƒŠƒtƒ@ƒNƒ^ƒŠƒ“ƒOÏ‚İ

/// <summary>
/// ƒ{ƒX“G‚É•K—v‚Èî•ñ
/// </summary>
public struct BossEnemySingletonData : IComponentData
{
    [Tooltip("PlayerTrackingNWay‚Ìn-Way’e")]
    public readonly NWay_DanmakuData playerTrackingNWay;

    [Tooltip("RotationNWay‚Ìn-Way’e")]
    public readonly NWay_DanmakuData rotationNWay;

    [Tooltip("RandomSpreadBullets‚Ìn-Way’e")]
    public readonly NWay_DanmakuData randomSpreadBulletsNWay;

    [Tooltip("RotationNWay‚ÌRotationData")]
    public readonly RotationData rotation;

    [Tooltip("UŒ‚‚ªØ‚è‘Ö‚í‚é‚Ü‚Å‚ÌŠÔ")]
    public readonly float attackSwitchTime;

    /// <summary>
    /// ƒ{ƒX“G‚Ìî•ñ
    /// </summary>
    /// <param name="playerTrackingNWay">PlayerTrackingNWay‚Ìn-Way’e‚Ìİ’è</param>
    /// <param name="rotationNWay">RotationNWay‚Ìn-Way’e‚Ìİ’è</param>
    /// <param name="randomSpreadBulletsNWay">RandomSpreadBullets‚Ìn-Way’e‚Ìİ’è</param>
    /// <param name="rotation">RotationNWay‚ÌRotationData</param>
    /// <param name="attackSwitchTime">UŒ‚‚ªØ‚è‘Ö‚í‚é‚Ü‚Å‚ÌŠÔ</param>
    public BossEnemySingletonData
    (
        NWay_DanmakuData playerTrackingNWay,
        NWay_DanmakuData rotationNWay,
        NWay_DanmakuData randomSpreadBulletsNWay,
        RotationData rotation,
        float attackSwitchTime
    )
    {
        // nWay’e
        this.playerTrackingNWay = playerTrackingNWay;
        this.rotationNWay = rotationNWay;
        this.randomSpreadBulletsNWay = randomSpreadBulletsNWay;

        // ‰ñ“]
        this.rotation = rotation;

        // UŒ‚‚ªØ‚è‘Ö‚í‚é‚Ü‚Å‚ÌŠÔ
        this.attackSwitchTime = attackSwitchTime;
    }
}

/// <summary>
/// ƒ{ƒX“G‚É•K—v‚Èİ’è
/// </summary>
public class BossEnemyAuthoring : SingletonMonoBehaviour<BossEnemyAuthoring>
{
    [SerializeField, Header("PlayerTrackingNWay‚Ìn-Way’e‚Ìİ’è")]
    private NWay_DanmakuSettingParameter _playerTrackingNWay = new();

    [SerializeField, Header("RotationNWay‚Ìn-Way’e‚Ìİ’è")]
    private NWay_DanmakuSettingParameter _rotationNWay = new();

    [SerializeField, Header("RandomSpreadBullets‚Ìn-Way’e‚Ìİ’è")]
    private NWay_DanmakuSettingParameter _randomSpreadBulletsNWay = new();

    [SerializeField, Header("©g‚Ì‰ñ“]‚É•K—v‚Èİ’è")]
    private RotationData _rotationData = new();

    [SerializeField, Min(0.0f), Header("UŒ‚‚ªØ‚è‘Ö‚í‚é‚Ü‚Å‚ÌŠÔ")]
    private float _attackSwitchTime = 0.0f;

    public class Baker : Baker<BossEnemyAuthoring>
    {
        public override void Bake(BossEnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Prefab‚ÌEntity‰»
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

            // Data‚ğƒAƒ^ƒbƒ`
            AddComponent(entity, bossEnemySingletonData);
        }
    }
}
