using Unity.Entities;
using Unity.Physics;
using static BulletHelper;
using static EntityCampsHelper;
using static HealthPointDatas;
using Unity.Transforms;

#if UNITY_EDITOR
using static EnemyHelper;
using static PlayerHelper;
using Unity.Burst;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using static HealthHelper;
using static PlayerAuthoring;
using static TriggerHelper;
using System;
#endif

/// <summary>
/// 接触した際の処理
/// </summary>
//[BurstCompile]
static public partial class TriggerJobs
{
    /// <summary>
    /// 接触時にダメージを受ける
    /// </summary>
    //[BurstCompile]
    public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
    {
        // インスタンスの取得に必要な変数
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;
        public ComponentLookup<RemainingPierceCountData> remainingPierceCountLookup;
        public ComponentLookup<LocalTransform> localTransformLookup;
        public ComponentLookup<VFXCreationData> vfxCreationLookup;
        public ComponentLookup<AudioPlayData> audioPlayLookup;

        // 現在の時刻
        public double currentTime;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // 接触対象
            var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

            // entityBがLocalTransformを有しているか
            if (!localTransformLookup.HasComponent(entityB)) { return; }

            // isTriggerが有効である以上、LocalTransformは絶対にある
            var localTransform = localTransformLookup[entityB];
            var position = localTransform.Position;

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
            if (dealDamage.campsType == EntityCampsType.Player) { return; }

            // ダメージを与え、変更されたインスタンスを反映する
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // ダメージ源がRemainingPierceCountDataを有していた
            if (remainingPierceCountLookup.HasComponent(entityA))
            {
                // 残り貫通回数をデクリメント
                var remainingPierceCount = remainingPierceCountLookup[entityA];
                remainingPierceCount.remainingPierceCount--;
                remainingPierceCountLookup[entityA] = remainingPierceCount;

                // ダメージ源がDestroyableDataを有していた
                if (destroyableLookup.HasComponent(entityA))
                {
                    var destroyable = destroyableLookup[entityA];

                    // 残り貫通回数が0以下か
                    destroyable.isKilled = (remainingPierceCount.remainingPierceCount <= 0) ? true : false;

                    // 変更を反映
                    destroyableLookup[entityA] = destroyable;
                }
            }

            // 攻撃を受けた対象がDestroyableDataを有していた
            if (destroyableLookup.HasComponent(entityB))
            {
                var destroyable = destroyableLookup[entityB];

                var isKilled = healthPoint.isKilled;

                // HPの削除フラグを代入する
                destroyable.isKilled = isKilled;

                // 変更を反映
                destroyableLookup[entityB] = destroyable;

                // 削除フラグが立った
                if (isKilled)
                {
                    // ゲームオーバー処理を開始する
                    GameManager.Instance.MyGameState = GameState.GameOver;

                    // EntityBがVFXCreationDataを有していた
                    if (vfxCreationLookup.HasComponent(entityB))
                    {
                        var vfxCreation = vfxCreationLookup[entityB];
                        vfxCreation.Position = position;
                        vfxCreationLookup[entityB] = vfxCreation;
                    }

                    // EntityBがAudioPlayDataを有していた
                    if (audioPlayLookup.HasComponent(entityB))
                    {
                        var audioPlay = audioPlayLookup[entityB];
                        audioPlay.AudioNumber = healthPoint.killedSENumber;
                        audioPlayLookup[entityB] = audioPlay;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 接触時にダメージを受ける
    /// </summary>
    //[BurstCompile]
    public partial struct EnemyDamageTriggerJob : ITriggerEventsJob
    {
        // インスタンスの取得に必要な変数
        public ComponentLookup<EnemyHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;
        public ComponentLookup<RemainingPierceCountData> remainingPierceCountLookup;
        public ComponentLookup<LocalTransform> localTransformLookup;
        public ComponentLookup<VFXCreationData> vfxCreationLookup;
        public ComponentLookup<AudioPlayData> audioPlayLookup;
        public ComponentLookup<BossEnemyCampsTag> bossEnemyCampsLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // 接触対象
            var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

            // entityBがLocalTransformを有しているか
            if (!localTransformLookup.HasComponent(entityB)) { return; }

            var localTransform = localTransformLookup[entityB];
            var position = localTransform.Position;

            // entityAがBulletIDealDamageDataを有していない。
            // あるいは、entityBがEnemyHealthPointDataを有していなければ切り上げる
            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            // entityBからEnemyHealthPointDataを取得
            var healthPoint = healthPointLookup[entityB];

            // entityAからBulletIDealDamageDataを取得
            var dealDamage = dealDamageLookup[entityA];

            // ダメージ源の陣営の種類がEnemyだったら切り上げる
            if (dealDamage.campsType == EntityCampsType.Enemy) { return; }

            // ダメージを与え、変更されたインスタンスを反映する
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // ダメージ源がRemainingPierceCountDataを有していた
            if (remainingPierceCountLookup.HasComponent(entityA))
            {
                // 残り貫通回数をデクリメント
                var remainingPierceCount = remainingPierceCountLookup[entityA];
                remainingPierceCount.remainingPierceCount--;
                remainingPierceCountLookup[entityA] = remainingPierceCount;

                // ダメージ源がDestroyableDataを有していた
                if (destroyableLookup.HasComponent(entityA))
                {
                    var destroyable = destroyableLookup[entityA];

                    // 残り貫通回数が0以下か
                    destroyable.isKilled = (remainingPierceCount.remainingPierceCount <= 0) ? true : false;

                    // 変更を反映
                    destroyableLookup[entityA] = destroyable;
                }
            }

            // 攻撃を受けた対象がDestroyableDataを有していた
            if (destroyableLookup.HasComponent(entityB))
            {
                var destroyable = destroyableLookup[entityB];

                var isKilled = healthPoint.isKilled;

                // HPの削除フラグを代入する
                destroyable.isKilled = isKilled;

                // 変更を反映
                destroyableLookup[entityB] = destroyable;

                // EntityBがAudioPlayDataを有していた
                if (audioPlayLookup.HasComponent(entityB))
                {
                    var audioPlay = audioPlayLookup[entityB];

                    // 削除フラグが立っていたら死亡時の効果音、立っていなければ被弾時の効果音を代入
                    audioPlay.AudioNumber = (isKilled) ? healthPoint.killedSENumber : healthPoint.hitSENumber;

                    audioPlayLookup[entityB] = audioPlay;
                }

                // 倒された
                if (isKilled)
                {
                    // EntityBがVFXCreationDataを有していた
                    if (vfxCreationLookup.HasComponent(entityB))
                    {
                        var vfxCreation = vfxCreationLookup[entityB];

                        vfxCreation.Position = position;
                        vfxCreationLookup[entityB] = vfxCreation;
                    }

                    // ボス敵だった
                    if (bossEnemyCampsLookup.HasComponent(entityB))
                    {
                        // ボスが倒されたためゲームクリア
                        GameManager.Instance.MyGameState = GameState.GameClear;
                    }
                }
            }
        }
    }
}
