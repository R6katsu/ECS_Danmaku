using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;
using System;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// タップ撃ち弾に必要なパラメータの設定
/// </summary>
[Serializable]
public struct TapShootingSettingParameter
{
    [SerializeField, Header("ワンセットでn回撃つ")]
    private int _shootNSingleSet;

    [SerializeField, Header("ワンセット終了後の休息時間")]
    private float _singleSetRestTimeAfter;

    [SerializeField, Header("発射間隔")]
    private float _firingInterval;

    [SerializeField, Header("弾のPrefab")]
    private Transform _bulletPrefab;

    /// <summary>
    /// ワンセットでn回撃つ
    /// </summary>
    public int ShootNSingleSet => _shootNSingleSet;

    /// <summary>
    /// ワンセット終了後の休息時間
    /// </summary>
    public float SingleSetRestTimeAfter => _singleSetRestTimeAfter;

    /// <summary>
    /// 発射間隔
    /// </summary>
    public float FiringInterval => _firingInterval;

    /// <summary>
    /// 弾のPrefab
    /// </summary>
    public Transform BulletPrefab => _bulletPrefab;
}

/// <summary>
/// タップ撃ち弾幕に必要な情報
/// </summary>
public struct TapShooting_DanmakuData : IComponentData
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

        // 初期値
        currentShotCount = 0;
        singleSetNextTime = 0.0f;
        firingNextTime = 0.0f;
    }

    /// <summary>
    /// タップ撃ち弾幕の情報
    /// </summary>
    /// <param name="tapShootingSettingParameter">タップ撃ち弾に必要なパラメータの設定</param>
    /// <param name="bulletEntity">弾のPrefabEntity</param>
    public TapShooting_DanmakuData(TapShootingSettingParameter tapShootingSettingParameter, Entity bulletEntity)
    {
        this.shootNSingleSet = tapShootingSettingParameter.ShootNSingleSet;
        this.singleSetRestTimeAfter = tapShootingSettingParameter.SingleSetRestTimeAfter;
        this.firingInterval = tapShootingSettingParameter.FiringInterval;
        this.bulletEntity = bulletEntity;

        // 初期値
        currentShotCount = 0;
        singleSetNextTime = 0.0f;
        firingNextTime = 0.0f;
    }
}

/// <summary>
/// タップ撃ち弾幕に必要な設定
/// </summary>
#if UNITY_EDITOR
[RequireComponent(typeof(DanmakuTypeSetup))]
#endif
public class TapShooting_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField, Header("タップ撃ち弾に必要なパラメータの設定")]
    private TapShootingSettingParameter _tapShootingSettingParameter = new();

    public class Baker : Baker<TapShooting_DanmakuAuthoring>
    {
        public override void Bake(TapShooting_DanmakuAuthoring src)
        {
            var bulletPrefab = GetEntity(src._tapShootingSettingParameter.BulletPrefab, TransformUsageFlags.Dynamic);

            var tapShooting_DanmakuData = new TapShooting_DanmakuData
            (
                src._tapShootingSettingParameter,
                bulletPrefab
            );

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), tapShooting_DanmakuData);
        }
    }
}
