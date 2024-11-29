using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// ^bvΏeΜξρ
/// </summary>
public struct TapShooting_DanmakuData : IComponentData, IDataDeletion
{
    [Tooltip("ZbgΕnρΒ")]
    public readonly int shootNSingleSet;

    [Tooltip("ZbgIΉγΜx§Τ")]
    public readonly float singleSetRestTimeAfter;

    [Tooltip("­ΛΤu")]
    public readonly float firingInterval;

    [Tooltip("eΜPrefabEntity")]
    public readonly Entity bulletEntity;

    [Tooltip("»έΜΛρ")]
    public int currentShotCount;

    [Tooltip("ρΜZbgJn")]
    public double singleSetNextTime;

    [Tooltip("ρΜΛ")]
    public double firingNextTime;

    public bool IsDataDeletion { get; set; }

    /// <summary>
    /// ^bvΏeΜξρ
    /// </summary>
    /// <param name="shootNSingleSet">ZbgΕnρΒ</param>
    /// <param name="singleSetRestTimeAfter">ZbgIΉγΜx§Τ</param>
    /// <param name="firingInterval">­ΛΤu</param>
    /// <param name="bulletEntity">eΜPrefabEntity</param>
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
/// ^bvΏeΜέθ
/// </summary>
[RequireComponent(typeof(DanmakuTypeSetup))]
public class TapShooting_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField,Min(0), Header("ZbgΕnρΒ")]
    private int _shootNSingleSet = 0;

    [SerializeField, Min(0.0f), Header("ZbgIΉγΜx§Τ")]
    private float _singleSetRestTimeAfter = 0.0f;

    [SerializeField, Min(0.0f), Header("­ΛΤu")]
    private float _firingInterval = 0.0f;

    [SerializeField, Header("eΜPrefab")]
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
