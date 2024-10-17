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
    // EnemyDamageTriggerJobとPlayerDamageTriggerJobを纏められないだろうか
    // 敵に無敵時間以外の要素を与えて連続Hitしないように修正する
    // Playerも死亡時に削除されるようにする
    // 削除処理もHPではなく別の場所に作れないだろうか
    // 死亡時の演出をVFXGraphを作成して呼び出す
    // 範囲から出た弾や敵を削除する

    /* 以下の処理でKillDataを取得し、フラグを切り替える
     * 
     * EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
     * Entity myEntity = // 何らかの方法でエンティティを取得
     * MyComponentAccessor.GetComponentDataFromEntity(myEntity, entityManager);
     * 
     * SystemやJob以外の場所で上記の処理を多用するのは処理が重くなるのではないか
     * 一ヵ所で纏めてentityManagerを使った処理をする方が軽量なのではないか
     */



    /// <summary>
    /// 接触時にダメージを受ける
    /// </summary>
    public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
    {
        // インスタンスの取得に必要な変数
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;

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

            // ダメージ源の陣営の種類がPlayerだったら切り上げる
            if (dealDamage.campsType == CampsType.Player) { return; }

            // ダメージを与え、変更されたインスタンスを反映する
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // 攻撃を受けた対象が削除にDestroyableDataを有していなければ切り上げる
            if (!destroyableLookup.HasComponent(entityB)) { return; }

            var destroyable = destroyableLookup[entityB];

            // HPの削除フラグを代入する
            destroyable.isKilled = healthPoint.isKilled;

            // 変更を反映
            destroyableLookup[entityB] = destroyable;
        }
    }

    /// <summary>
    /// 接触時にダメージを受ける
    /// </summary>
    public partial struct EnemyDamageTriggerJob : ITriggerEventsJob
    {
        // インスタンスの取得に必要な変数
        public ComponentLookup<EnemyHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // 接触対象
            var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

            // entityAがBulletIDealDamageDataを有していない。
            // あるいは、entityBがEnemyHealthPointDataを有していなければ切り上げる
            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            // entityBからEnemyHealthPointDataを取得
            var healthPoint = healthPointLookup[entityB];

            // entityAからBulletIDealDamageDataを取得
            var dealDamage = dealDamageLookup[entityA];

            // ダメージ源の陣営の種類がEnemyだったら切り上げる
            if (dealDamage.campsType == CampsType.Enemy) { return; }

            // ダメージを与え、変更されたインスタンスを反映する
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // 攻撃を受けた対象が削除にDestroyableDataを有していなければ切り上げる
            if (!destroyableLookup.HasComponent(entityB)) { return; }

            var destroyable = destroyableLookup[entityB];

            // HPの削除フラグを代入する
            destroyable.isKilled = healthPoint.isKilled;

            // 変更を反映
            destroyableLookup[entityB] = destroyable;
        }
    }
}
