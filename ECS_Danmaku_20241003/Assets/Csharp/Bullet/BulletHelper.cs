using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

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
        // IDealDamage
        public void DealDamage(IHealthPoint healthPoint)
        {
            Debug.Log("IHealthPoint‚Öƒ_ƒ[ƒW‚ğ—^‚¦‚é");

            healthPoint.DamageHP(1.0f);
        }
    }
}
