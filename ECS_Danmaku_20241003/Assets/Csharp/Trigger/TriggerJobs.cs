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
using static TriggerHelper;

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

        public double currentTime;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // 接触対象
            var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

            // entityAがBulletIDealDamageDataを有していない。
            // あるいは、entityBがPlayerHealthPointDataを有していない
            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            // entityBからPlayerHealthPointDataを取得
            var healthPoint = healthPointLookup[entityB];

            // 前回被弾した時間から無敵時間の長さを経過していない
            if (currentTime - healthPoint.lastHitTime <= healthPoint.isInvincibleTime) { return; }

            // entityAからBulletIDealDamageDataを取得
            var dealDamage = dealDamageLookup[entityA];

            healthPointLookup[entityB] = dealDamage.DealDamage(healthPoint, currentTime);
        }
    }
}
