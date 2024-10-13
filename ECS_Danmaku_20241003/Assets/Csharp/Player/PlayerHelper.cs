using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;

static public class PlayerHelper
{
    /// <summary>
    /// Player
    /// </summary>
    public struct PlayerTag : IComponentData { }

    /// <summary>
    /// PlayerのIHealthPoint情報
    /// </summary>
    public struct PlayerHealthPointData : IComponentData, IHealthPoint
    {
        // IHealthPoint
        public void DamageHP(float damage)
        {
            Debug.Log("ダメージを受ける");
        }

        // IHealthPoint
        public void HealHP(float heal)
        {
            Debug.Log("HPを回復する");
        }
    }
}
