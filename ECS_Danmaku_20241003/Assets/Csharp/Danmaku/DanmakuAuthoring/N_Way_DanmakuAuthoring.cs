using System;
using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
#endif

// ƒŠƒtƒ@ƒNƒ^ƒŠƒ“ƒOÏ‚İ

/// <summary>
/// Way’e‚É•K—v‚Èƒpƒ‰ƒ[ƒ^‚Ìİ’è
/// </summary>
[Serializable]
public struct NWay_DanmakuSettingParameter
{
    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("î‚Ì‘å‚«‚³")]
    private int _fanAngle;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("’e‚Ì—Ê")]
    private int _amountBullets;

    [SerializeField, Header("”­ËŠÔŠu")]
    private float _firingInterval;

    [SerializeField, Header("’e‚ÌEntity")]
    private Transform _bulletPrefab;

    [Tooltip("Œo‰ßŠÔ")]
    private float _elapsedTime;

    /// <summary>
    /// î‚Ì‘å‚«‚³
    /// </summary>
    public int FanAngle =>_fanAngle;

    /// <summary>
    /// ’e‚Ì—Ê
    /// </summary>
    public int AmountBullets => _amountBullets;

    /// <summary>
    /// ”­ËŠÔŠu
    /// </summary>
    public float FiringInterval => _firingInterval;

    /// <summary>
    /// ’e‚ÌEntity
    /// </summary>
    public Transform BulletPrefab => _bulletPrefab;

    /// <summary>
    /// Œo‰ßŠÔ
    /// </summary>
    public float ElapsedTime =>_elapsedTime;
}

/// <summary>
/// n-Way’e‚É•K—v‚Èî•ñ
/// </summary>
public struct NWay_DanmakuData : IComponentData
{
    [Tooltip("î‚Ì‘å‚«‚³")]
    public readonly int fanAngle;

    [Tooltip("’e‚Ì—Ê")]
    public readonly int amountBullets;

    [Tooltip("”­ËŠÔŠu")]
    public readonly float firingInterval;

    [Tooltip("’e‚ÌEntity")]
    public readonly Entity bulletEntity;

    [Tooltip("Œo‰ßŠÔ")]
    public float elapsedTime;

    /// <summary>
    /// n-Way’e
    /// </summary>
    /// <param name="fanAngle">î‚Ì‘å‚«‚³</param>
    /// <param name="amountBullets">’e‚Ì—Ê</param>
    /// <param name="firingInterval">”­ËŠÔŠu</param>
    /// <param name="bulletPrefab">’e‚ÌPrefab</param>
    public NWay_DanmakuData(int fanAngle, int amountBullets, float firingInterval, Entity bulletPrefab)
    {
        this.fanAngle = fanAngle;
        this.amountBullets = amountBullets;
        this.firingInterval = firingInterval;
        this.bulletEntity = bulletPrefab;

        // ‰Šú’l
        elapsedTime = 0.0f;
    }

    /// <summary>
    /// n-Way’e
    /// </summary>
    /// <param name="nWay_DanmakuSettingParam">Way’e‚É•K—v‚Èƒpƒ‰ƒ[ƒ^‚Ìİ’è</param>
    /// <param name="bulletPrefab">’e‚ÌPrefab</param>
    public NWay_DanmakuData(NWay_DanmakuSettingParameter nWay_DanmakuSettingParam, Entity bulletPrefab)
    {
        this.fanAngle = nWay_DanmakuSettingParam.FanAngle;
        this.amountBullets = nWay_DanmakuSettingParam.AmountBullets;
        this.firingInterval = nWay_DanmakuSettingParam.FiringInterval;
        this.bulletEntity = bulletPrefab;

        // ‰Šú’l
        elapsedTime = 0.0f;
    }
}

/// <summary>
/// n-Way’e‚É•K—v‚Èİ’è
/// </summary>
#if UNITY_EDITOR
[RequireComponent(typeof(DanmakuTypeSetup))]
#endif
public class N_Way_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField, Header("Way’e‚É•K—v‚Èƒpƒ‰ƒ[ƒ^‚Ìİ’è")]
    private NWay_DanmakuSettingParameter _nWay_DanmakuSettingParameter = new();

    public class Baker : Baker<N_Way_DanmakuAuthoring>
    {
        public override void Bake(N_Way_DanmakuAuthoring src)
        {
            var bulletEntity = GetEntity(src._nWay_DanmakuSettingParameter.BulletPrefab, TransformUsageFlags.Dynamic);

            var n_Way_DanmakuData = new NWay_DanmakuData
            (
                src._nWay_DanmakuSettingParameter,
                bulletEntity
            );

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), n_Way_DanmakuData);
        }
    }
}
