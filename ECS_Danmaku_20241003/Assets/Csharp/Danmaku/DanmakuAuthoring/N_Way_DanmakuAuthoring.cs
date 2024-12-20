using System;
using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
#endif

// リファクタリング済み

/// <summary>
/// Way弾に必要なパラメータの設定
/// </summary>
[Serializable]
public struct NWay_DanmakuSettingParameter
{
    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("扇の大きさ")]
    private int _fanAngle;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("弾の量")]
    private int _amountBullets;

    [SerializeField, Header("発射間隔")]
    private float _firingInterval;

    [SerializeField, Header("弾のEntity")]
    private Transform _bulletPrefab;

    [Tooltip("経過時間")]
    private float _elapsedTime;

    /// <summary>
    /// 扇の大きさ
    /// </summary>
    public int FanAngle =>_fanAngle;

    /// <summary>
    /// 弾の量
    /// </summary>
    public int AmountBullets => _amountBullets;

    /// <summary>
    /// 発射間隔
    /// </summary>
    public float FiringInterval => _firingInterval;

    /// <summary>
    /// 弾のEntity
    /// </summary>
    public Transform BulletPrefab => _bulletPrefab;

    /// <summary>
    /// 経過時間
    /// </summary>
    public float ElapsedTime =>_elapsedTime;
}

/// <summary>
/// n-Way弾に必要な情報
/// </summary>
public struct NWay_DanmakuData : IComponentData
{
    [Tooltip("扇の大きさ")]
    public readonly int fanAngle;

    [Tooltip("弾の量")]
    public readonly int amountBullets;

    [Tooltip("発射間隔")]
    public readonly float firingInterval;

    [Tooltip("弾のEntity")]
    public readonly Entity bulletEntity;

    [Tooltip("経過時間")]
    public float elapsedTime;

    /// <summary>
    /// n-Way弾
    /// </summary>
    /// <param name="fanAngle">扇の大きさ</param>
    /// <param name="amountBullets">弾の量</param>
    /// <param name="firingInterval">発射間隔</param>
    /// <param name="bulletPrefab">弾のPrefab</param>
    public NWay_DanmakuData(int fanAngle, int amountBullets, float firingInterval, Entity bulletPrefab)
    {
        this.fanAngle = fanAngle;
        this.amountBullets = amountBullets;
        this.firingInterval = firingInterval;
        this.bulletEntity = bulletPrefab;

        // 初期値
        elapsedTime = 0.0f;
    }

    /// <summary>
    /// n-Way弾
    /// </summary>
    /// <param name="nWay_DanmakuSettingParam">Way弾に必要なパラメータの設定</param>
    /// <param name="bulletPrefab">弾のPrefab</param>
    public NWay_DanmakuData(NWay_DanmakuSettingParameter nWay_DanmakuSettingParam, Entity bulletPrefab)
    {
        this.fanAngle = nWay_DanmakuSettingParam.FanAngle;
        this.amountBullets = nWay_DanmakuSettingParam.AmountBullets;
        this.firingInterval = nWay_DanmakuSettingParam.FiringInterval;
        this.bulletEntity = bulletPrefab;

        // 初期値
        elapsedTime = 0.0f;
    }
}

/// <summary>
/// n-Way弾に必要な設定
/// </summary>
#if UNITY_EDITOR
[RequireComponent(typeof(DanmakuTypeSetup))]
#endif
public class N_Way_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField, Header("Way弾に必要なパラメータの設定")]
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
