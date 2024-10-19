using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EntityCampsHelper;
using static HealthHelper;
using static MoveHelper;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// íeÇÃï‚èï
/// </summary>
static public class BulletHelper
{
    /// <summary>
    /// íe
    /// </summary>
    public struct BulletTag : IComponentData { }

    /// <summary>
    /// íeÇÃIDealDamageèÓïÒ
    /// </summary>
    public struct BulletIDealDamageData : IComponentData, IDealDamage
    {
        public readonly float damageAmount;
        public readonly EntityCampsType campsType;

        /// <summary>
        /// íeÇÃIDealDamageèÓïÒ
        /// </summary>
        /// <param name="damageAmount">É_ÉÅÅ[ÉWó </param>
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
