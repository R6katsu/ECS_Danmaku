using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EntityCampsHelper;
using static HealthHelper;

#if UNITY_EDITOR
using static MoveHelper;
using static UnityEngine.EventSystems.EventTrigger;
#endif

/// <summary>
/// ’e‚Ì•â•
/// </summary>
static public class BulletHelper
{
    /// <summary>
    /// ’e
    /// </summary>
    public struct BulletTag : IComponentData { }

    /// <summary>
    /// c‚èŠÑ’Ê‰ñ”‚Ìî•ñ
    /// </summary>
    public struct RemainingPierceCountData : IComponentData
    {
        public int remainingPierceCount;

        /// <summary>
        /// c‚èŠÑ’Ê‰ñ”‚Ìî•ñ
        /// </summary>
        /// <param name="remainingPierceCount">c‚èŠÑ’Ê‰ñ”</param>
        public RemainingPierceCountData(int remainingPierceCount)
        {
            this.remainingPierceCount = remainingPierceCount;
        }
    }

    /// <summary>
    /// ’e‚ÌIDealDamageî•ñ
    /// </summary>
    public struct BulletIDealDamageData : IComponentData, IDealDamage
    {
        public readonly float damageAmount;
        public readonly EntityCampsType campsType;

        /// <summary>
        /// ’e‚ÌIDealDamageî•ñ
        /// </summary>
        /// <param name="damageAmount">ƒ_ƒ[ƒW—Ê</param>
        public BulletIDealDamageData(float damageAmount, EntityCampsType campsType)
        {
            this.damageAmount = damageAmount;
            this.campsType = campsType;
        }

        // IDealDamage
        public T DealDamage<T>(T healthPoint, Entity entity) where T : IHealthPoint
        {
            healthPoint.DamageHP(damageAmount, entity);
            return healthPoint;
        }
    }
}
