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
/// n-Way’e
/// </summary>
public struct N_Way_DanmakuData : IComponentData, IEnumerable//, IDataDeletion
{
    [Tooltip("î‚Ì‘å‚«‚³")]
    public readonly int fanAngle;

    [Tooltip("’e‚Ì—Ê")]
    public readonly int amountBullets;

    [Tooltip("”­ËŠÔŠu")]
    public readonly float firingInterval;

    [Tooltip("’e‚ÌEntity")]
    public readonly Entity bulletEntity;

    [Tooltip("’e‚ÌEntity‚Ì‘å‚«‚³")]
    public readonly float bulletLocalScale;

    [Tooltip("Œo‰ßŠÔ")]
    public float elapsedTime;

    public bool IsDataDeletion;

    public N_Way_DanmakuData aaaa()
    {
        this.IsDataDeletion = true;
        return this;
    }

    /// <summary>
    /// n-Way’e
    /// </summary>
    /// <param name="fanAngle">î‚Ì‘å‚«‚³</param>
    /// <param name="amountBullets">’e‚Ì—Ê</param>
    /// <param name="firingInterval">”­ËŠÔŠu</param>
    /// <param name="bulletPrefab">’e‚ÌPrefab</param>
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
/// n-Way’e‚Ìİ’è
/// </summary>
[RequireComponent(typeof(DanmakuTypeSetup))]
public class N_Way_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("î‚Ì‘å‚«‚³")]
    private int _fanAngle = 0;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("’e‚Ì—Ê")]
    private int _amountBullets = 0;

    [SerializeField, Min(0.0f), Header("”­ËŠÔŠu")]
    private float _firingInterval = 0.0f;

    [SerializeField, Header("’e‚ÌPrefab")]
    private Transform _bulletPrefab = null;

    public class Baker : Baker<N_Way_DanmakuAuthoring>
    {
        public override void Bake(N_Way_DanmakuAuthoring src)
        {
            var bulletEntity = GetEntity(src._bulletPrefab, TransformUsageFlags.Dynamic);

            // LocalTransform‚ÌScale‚ªfloat‚Å‚ ‚éˆ×Aˆê”Ô‘å‚«‚¢²‚ğ‹‚ß‚é‚±‚Æ‚É‚µ‚½
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
