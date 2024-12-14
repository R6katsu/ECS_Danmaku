using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static EntityCampsHelper;
using static HealthPointDatas;
using static BulletHelper;
using Unity.Physics;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// EnemyTagを有するEntityの処理
/// </summary>
[BurstCompile]
public partial struct EnemySystem : ISystem
{
    private ComponentLookup<HealthPointData> _healthPointLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;
    private ComponentLookup<RemainingPierceCountData> _remainingPierceCountLookup;
    private ComponentLookup<LocalTransform> _localTransformLookup;
    private ComponentLookup<VFXCreationData> _vfxCreationLookup;
    private ComponentLookup<AudioPlayData> _audioPlayLookup;
    private ComponentLookup<BossEnemyCampsTag> _bossEnemyCampsLookup;

    public void OnCreate(ref SystemState state)
    {
        // 取得する
        _healthPointLookup = state.GetComponentLookup<HealthPointData>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
        _remainingPierceCountLookup = state.GetComponentLookup<RemainingPierceCountData>(false);
        _localTransformLookup = state.GetComponentLookup<LocalTransform>(false);
        _vfxCreationLookup = state.GetComponentLookup<VFXCreationData>(false);
        _audioPlayLookup = state.GetComponentLookup<AudioPlayData>(false);
        _bossEnemyCampsLookup = state.GetComponentLookup<BossEnemyCampsTag>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        // MovementRangeSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // シングルトンデータの取得
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // 敵の移動可能範囲を取得
        var enemyMovementRange = movementRangeSingleton.EnemyMovementRange;

        // 中心位置
        var movementRangeCenter = enemyMovementRange.movementRangeCenter;

        // 半分の大きさを求める
        var halfMovementRange = enemyMovementRange.movementRange.Halve();

        // 中心位置を考慮した移動可能範囲を求める
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // 移動可能範囲外だったら削除フラグを立てる
        foreach (var (enemy, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<EnemyCampsTag>,
                 RefRW<DestroyableData>,
                 RefRW<LocalTransform>>())
        {
            var position = localTfm.ValueRO.Position;

            // 移動可能範囲外だった
            if (position.x < minMovementRange.x || position.x > maxMovementRange.x ||
                position.y < minMovementRange.y || position.y > maxMovementRange.y ||
                position.z < minMovementRange.z || position.z > maxMovementRange.z)
            {
                // 削除フラグを立てる
                destroyable.ValueRW.isKilled = true;
            }
        }

        // 敵の被弾処理
        EnemyDamageTriggerJob(ref state);
    }

    /// <summary>
    /// 敵の被弾処理を呼び出す
    /// </summary>
    /// <param name="state"></param>
    private void EnemyDamageTriggerJob(ref SystemState state)
    {
        // 更新する
        _healthPointLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);
        _remainingPierceCountLookup.Update(ref state);
        _localTransformLookup.Update(ref state);
        _vfxCreationLookup.Update(ref state);
        _audioPlayLookup.Update(ref state);
        _bossEnemyCampsLookup.Update(ref state);

        int frameCount = Time.frameCount;

        // 敵に弾が当たった時の処理を呼び出す
        var enemyDamage = new EnemyDamageTriggerJob()
        {
            healthPointLookup = _healthPointLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            remainingPierceCountLookup = _remainingPierceCountLookup,
            localTransformLookup = _localTransformLookup,
            vfxCreationLookup = _vfxCreationLookup,
            audioPlayLookup = _audioPlayLookup,
            bossEnemyCampsLookup = _bossEnemyCampsLookup,
            frameCount = frameCount
        };

        // 前のジョブを完了する
        state.Dependency.Complete();

        var enemyJobHandle = enemyDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = enemyJobHandle;
    }
}

/// <summary>
/// 接触時にダメージを受ける
/// </summary>
public partial struct EnemyDamageTriggerJob : ITriggerEventsJob
{
    // インスタンスの取得に必要な変数
    public ComponentLookup<HealthPointData> healthPointLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;
    public ComponentLookup<RemainingPierceCountData> remainingPierceCountLookup;
    public ComponentLookup<LocalTransform> localTransformLookup;
    public ComponentLookup<VFXCreationData> vfxCreationLookup;
    public ComponentLookup<AudioPlayData> audioPlayLookup;
    public ComponentLookup<BossEnemyCampsTag> bossEnemyCampsLookup;

    [Tooltip("接触したフレーム")]
    public int frameCount;

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
        healthPoint = dealDamage.DealDamage(healthPoint, entityA, frameCount);
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

            // HPの削除フラグを代入する
            destroyable.isKilled = healthPoint.CurrentHP <= 0;

            // 変更を反映
            destroyableLookup[entityB] = destroyable;

            // EntityBがAudioPlayDataを有していた
            if (audioPlayLookup.HasComponent(entityB))
            {
                var audioPlay = audioPlayLookup[entityB];

                // 削除フラグが立っていたら死亡時の効果音、立っていなければ被弾時の効果音を代入
                audioPlay.AudioNumber = (destroyable.isKilled) ? healthPoint.killedSENumber : healthPoint.hitSENumber;

                audioPlayLookup[entityB] = audioPlay;
            }

            // 倒された
            if (destroyable.isKilled)
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