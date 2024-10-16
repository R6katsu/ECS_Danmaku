using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

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
    /// ’e‚ÌIDealDamageî•ñ
    /// </summary>
    public struct BulletIDealDamageData : IComponentData, IDealDamage
    {
        public readonly float damageAmount;
        public readonly CampsType campsType;

        /// <summary>
        /// ’e‚ÌIDealDamageî•ñ
        /// </summary>
        /// <param name="damageAmount">ƒ_ƒ[ƒW—Ê</param>
        public BulletIDealDamageData(float damageAmount, CampsType campsType)
        {
            this.damageAmount = damageAmount;
            this.campsType = campsType;
        }

        // IDealDamage
        public T DealDamage<T>(T healthPoint) where T : IHealthPoint
        {
            healthPoint.DamageHP(damageAmount);
            return healthPoint;
        }
    }
}
