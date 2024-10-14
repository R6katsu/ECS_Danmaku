using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

static public class BulletHelper
{
    /// <summary>
    /// �e
    /// </summary>
    public struct BulletTag : IComponentData { }

    /// <summary>
    /// �e��IDealDamage���
    /// </summary>
    public struct BulletIDealDamageData : IComponentData, IDealDamage
    {
        public readonly float damageAmount;

        public BulletIDealDamageData(float damageAmount)
        {
            this.damageAmount = damageAmount;
        }

        // IDealDamage
        public T DealDamage<T>(T healthPoint, double currentTime) where T : IHealthPoint
        {
            healthPoint.DamageHP(damageAmount, currentTime);
            return healthPoint;
        }
    }
}
