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
/// 接触した際の処理
/// </summary>
static public partial class TriggerJobs
{
    /// <summary>
    /// 接触時にダメージを受ける
    /// </summary>
    public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
    {
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // 接触対象
            var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            var dealDamage = dealDamageLookup[entityA];
            var healthPoint = healthPointLookup[entityB];

            dealDamage.DealDamage(healthPoint);
        }
    }
}
