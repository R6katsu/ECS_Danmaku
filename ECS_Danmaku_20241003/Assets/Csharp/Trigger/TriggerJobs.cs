using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using static BulletHelper;
using static EnemyHelper;
using static HealthHelper;
using static PlayerAuthoring;
using static PlayerHelper;

/// <summary>
/// ÚG‚µ‚½Û‚Ìˆ—
/// </summary>
static public partial class TriggerJobs
{
    /// <summary>
    /// ÚG‚Éƒ_ƒ[ƒW‚ğó‚¯‚é
    /// </summary>
    public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
    {
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // ÚG‘ÎÛ
            var entityB = triggerEvent.EntityB; // isTrigger‚ğ—LŒø‚É‚µ‚Ä‚¢‚é•û

            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            var dealDamage = dealDamageLookup[entityA];
            var healthPoint = healthPointLookup[entityB];

            dealDamage.DealDamage(healthPoint);
        }
    }
}
