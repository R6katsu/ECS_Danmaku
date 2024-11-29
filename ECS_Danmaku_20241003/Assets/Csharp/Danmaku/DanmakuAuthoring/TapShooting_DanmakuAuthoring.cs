using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// タップ撃ち弾幕の情報
/// </summary>
public struct TapShooting_DanmakuData : IComponentData, IDataDeletion
{
    [Tooltip("ワンセットでn回撃つ")]
    public readonly int shootNSingleSet;

    [Tooltip("ワンセット終了後の休息時間")]
    public readonly float singleSetRestTimeAfter;

    [Tooltip("発射間隔")]
    public readonly float firingInterval;

    [Tooltip("弾のPrefabEntity")]
    public readonly Entity bulletEntity;

    [Tooltip("現在の射撃回数")]
    public int currentShotCount;

    [Tooltip("次回のワンセット開始時刻")]
    public double singleSetNextTime;

    [Tooltip("次回の射撃時刻")]
    public double firingNextTime;

    public bool IsDataDeletion { get; set; }

    /// <summary>
    /// タップ撃ち弾幕の情報
    /// </summary>
    /// <param name="shootNSingleSet">ワンセットでn回撃つ</param>
    /// <param name="singleSetRestTimeAfter">ワンセット終了後の休息時間</param>
    /// <param name="firingInterval">発射間隔</param>
    /// <param name="bulletEntity">弾のPrefabEntity</param>
    public TapShooting_DanmakuData(int shootNSingleSet, float singleSetRestTimeAfter, float firingInterval, Entity bulletEntity)
    {
        this.shootNSingleSet = shootNSingleSet;
        this.singleSetRestTimeAfter = singleSetRestTimeAfter;
        this.firingInterval = firingInterval;
        this.bulletEntity = bulletEntity;

        currentShotCount = 0;
        singleSetNextTime = 0.0f;
        firingNextTime = 0.0f;

        IsDataDeletion = false;
    }
}

/// <summary>
/// タップ撃ち弾幕の設定
/// </summary>
[RequireComponent(typeof(DanmakuTypeSetup))]
public class TapShooting_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField,Min(0), Header("ワンセットでn回撃つ")]
    private int _shootNSingleSet = 0;

    [SerializeField, Min(0.0f), Header("ワンセット終了後の休息時間")]
    private float _singleSetRestTimeAfter = 0.0f;

    [SerializeField, Min(0.0f), Header("発射間隔")]
    private float _firingInterval = 0.0f;

    [SerializeField, Header("弾のPrefab")]
    private Transform _bulletPrefab = null;

    public class Baker : Baker<TapShooting_DanmakuAuthoring>
    {
        public override void Bake(TapShooting_DanmakuAuthoring src)
        {
            var bulletPrefab = GetEntity(src._bulletPrefab, TransformUsageFlags.Dynamic);

            var tapShooting_DanmakuData = new TapShooting_DanmakuData
                (
                    src._shootNSingleSet,
                    src._singleSetRestTimeAfter,
                    src._firingInterval,
                    bulletPrefab
                );
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), tapShooting_DanmakuData);
        }
    }
}
