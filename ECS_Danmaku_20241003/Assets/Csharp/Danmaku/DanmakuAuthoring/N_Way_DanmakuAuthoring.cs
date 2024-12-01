using System;
using Unity.Entities;
using System.Collections;
using UnityEngine;
using static DanmakuHelper;

#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Mathematics;
#endif

/// <summary>
/// n-Way弾
/// </summary>
public struct N_Way_DanmakuData : IComponentData, IEnumerable//, IDataDeletion
{
    [Tooltip("扇の大きさ")]
    public readonly int fanAngle;

    [Tooltip("弾の量")]
    public readonly int amountBullets;

    [Tooltip("発射間隔")]
    public readonly float firingInterval;

    [Tooltip("弾のEntity")]
    public readonly Entity bulletEntity;

    [Tooltip("弾のEntityの大きさ")]
    public readonly float bulletLocalScale;

    [Tooltip("経過時間")]
    public float elapsedTime;

    public bool IsDataDeletion;

    public N_Way_DanmakuData aaaa()
    {
        this.IsDataDeletion = true;
        return this;
    }

    /// <summary>
    /// n-Way弾
    /// </summary>
    /// <param name="fanAngle">扇の大きさ</param>
    /// <param name="amountBullets">弾の量</param>
    /// <param name="firingInterval">発射間隔</param>
    /// <param name="bulletPrefab">弾のPrefab</param>
    public N_Way_DanmakuData(int fanAngle, int amountBullets, float firingInterval, Entity bulletPrefab, float bulletLocalScale)
    {
        this.fanAngle = fanAngle;
        this.amountBullets = amountBullets;
        this.firingInterval = firingInterval;
        this.bulletEntity = bulletPrefab;
        this.bulletLocalScale = bulletLocalScale;
        elapsedTime = 0.0f;
        IsDataDeletion = false;
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// n-Way弾の設定
/// </summary>
[RequireComponent(typeof(DanmakuTypeSetup))]
public class N_Way_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("扇の大きさ")]
    private int _fanAngle = 0;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("弾の量")]
    private int _amountBullets = 0;

    [SerializeField, Min(0.0f), Header("発射間隔")]
    private float _firingInterval = 0.0f;

    [SerializeField, Header("弾のPrefab")]
    private Transform _bulletPrefab = null;

    public class Baker : Baker<N_Way_DanmakuAuthoring>
    {
        public override void Bake(N_Way_DanmakuAuthoring src)
        {
            var bulletEntity = GetEntity(src._bulletPrefab, TransformUsageFlags.Dynamic);

            // LocalTransformのScaleがfloatである為、一番大きい軸を求めることにした
            var localScale = src._bulletPrefab.localScale;
            var localScaleMax = Mathf.Max(localScale.x, localScale.y, localScale.z);

            var n_Way_DanmakuData = new N_Way_DanmakuData
                (
                    src._fanAngle,
                    src._amountBullets,
                    src._firingInterval,
                    bulletEntity,
                    localScaleMax
                );

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), n_Way_DanmakuData);
        }
    }
}
