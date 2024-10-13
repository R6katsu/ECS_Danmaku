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
    /// Player‚ÌIHealthPointî•ñ
    /// </summary>
    public struct PlayerHealthPointData : IComponentData, IHealthPoint
    {
        // IHealthPoint
        public void DamageHP(float damage)
        {
            Debug.Log("ƒ_ƒ[ƒW‚ğó‚¯‚é");
        }

        // IHealthPoint
        public void HealHP(float heal)
        {
            Debug.Log("HP‚ğ‰ñ•œ‚·‚é");
        }
    }
}
