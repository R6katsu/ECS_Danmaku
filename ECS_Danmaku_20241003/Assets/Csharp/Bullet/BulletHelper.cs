using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

static public class BulletHelper
{
    /// <summary>
    /// 弾
    /// </summary>
    public struct BulletTag : IComponentData { }

    /// <summary>
    /// 弾のIDealDamage情報
    /// </summary>
    public struct BulletIDealDamageData : IComponentData, IDealDamage
    {
        // IDealDamage
        public void DealDamage(IHealthPoint healthPoint)
        {
            Debug.Log("IHealthPointへダメージを与える");

            healthPoint.DamageHP(1.0f);
        }
    }
}
