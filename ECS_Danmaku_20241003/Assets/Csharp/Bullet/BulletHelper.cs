using Unity.Entities;
using static HealthHelper;

#if UNITY_EDITOR
using static EntityCampsHelper;
using static MoveHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
#endif

// リファクタリング済み

/// <summary>
/// 弾の補助
/// </summary>
static public class BulletHelper
{
    /// <summary>
    /// 弾
    /// </summary>
    public struct BulletTag : IComponentData { }

    /// <summary>
    /// 残り貫通回数の情報
    /// </summary>
    public struct RemainingPierceCountData : IComponentData
    {
        public int remainingPierceCount;

        /// <summary>
        /// 残り貫通回数の情報
        /// </summary>
        /// <param name="remainingPierceCount">残り貫通回数</param>
        public RemainingPierceCountData(int remainingPierceCount)
        {
            this.remainingPierceCount = remainingPierceCount;
        }
    }

    /// <summary>
    /// 弾のIDealDamage情報
    /// </summary>
    public struct BulletIDealDamageData : IComponentData, IDealDamage
    {
        public readonly float damageAmount;
        public readonly EntityCampsType campsType;

        /// <summary>
        /// 弾のIDealDamage情報
        /// </summary>
        /// <param name="damageAmount">ダメージ量</param>
        public BulletIDealDamageData(float damageAmount, EntityCampsType campsType)
        {
            this.damageAmount = damageAmount;
            this.campsType = campsType;
        }

        // IDealDamage
        public T DealDamage<T>(T healthPoint, Entity entity, int frameCount) where T : IHealthPoint
        {
            healthPoint.DamageHP(damageAmount, entity, frameCount);

            // IHealthPointのインスタンス更新の為に返す
            return healthPoint;
        }
    }
}
