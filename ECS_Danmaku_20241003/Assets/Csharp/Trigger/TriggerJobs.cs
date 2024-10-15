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
        // インスタンスの取得に必要な変数
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;

        // 現在の時刻
        public double currentTime;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // 接触対象
            var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

            // entityAがBulletIDealDamageDataを有していない。
            // あるいは、entityBがPlayerHealthPointDataを有していなければ切り上げる
            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            // entityBからPlayerHealthPointDataを取得
            var healthPoint = healthPointLookup[entityB];

            // 前回被弾した時間から無敵時間の長さを経過していない
            if (currentTime - healthPoint.lastHitTime <= healthPoint.isInvincibleTime) { return; }

            // entityAからBulletIDealDamageDataを取得
            var dealDamage = dealDamageLookup[entityA];

            // ダメージを与え、変更されたインスタンスを反映する
            healthPointLookup[entityB] = dealDamage.DealDamage(healthPoint, currentTime);
        }
    }
}
