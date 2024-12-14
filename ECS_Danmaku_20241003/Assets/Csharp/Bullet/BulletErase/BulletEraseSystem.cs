using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using static BulletHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using static EntityCampsHelper;
using static HealthPointDatas;
#endif

// リファクタリング済み

/// <summary>
/// 弾を消す職能の処理
/// </summary>
[BurstCompile]
public partial struct BulletEraseSystem : ISystem
{
    private ComponentLookup<BulletEraseTag> _bulletEraseLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;

    public void OnCreate(ref SystemState state)
    {
        // 取得する
        _bulletEraseLookup = state.GetComponentLookup<BulletEraseTag>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        // 更新する
        _bulletEraseLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);

        // 弾を消す職能の衝突時の処理を呼び出す
        var bulletErase = new BulletEraseTriggerJob()
        {
            bulletEraseLookup = _bulletEraseLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup
        };

        // 前のジョブを完了する
        state.Dependency.Complete();

        var bulletEraseJobHandle = bulletErase.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = bulletEraseJobHandle;
    }
}

/// <summary>
/// 弾を消す職能の衝突時の処理
/// </summary>
[BurstCompile]
public partial struct BulletEraseTriggerJob : ITriggerEventsJob
{
    public ComponentLookup<BulletEraseTag> bulletEraseLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // 接触対象
        var entityB = triggerEvent.EntityB; // isTriggerを有効にしている方

        // entityBがBulletEraseTagを有していない。
        if (!bulletEraseLookup.HasComponent(entityB)) { return; }

        // entityAがBulletIDealDamageDataを有していない。
        if (!dealDamageLookup.HasComponent(entityA)) { return; }

        // entityAからBulletIDealDamageDataを取得
        var dealDamage = dealDamageLookup[entityA];

        // entityBがBulletIDealDamageDataを有していた
        if (dealDamageLookup.HasComponent(entityB))
        {
            // entityBからBulletIDealDamageDataを取得
            var dealDamageEntityB = dealDamageLookup[entityB];

            // 同じ陣営だったら切り上げる
            if (dealDamage.campsType == dealDamageEntityB.campsType) { return; }
        }

        // 接触対象がDestroyableDataを有していた
        if (!destroyableLookup.HasComponent(entityA)) { return; }

        // 接触対象の削除フラグを立てる
        var destroyable = destroyableLookup[entityA];
        destroyable.isKilled = true;
        destroyableLookup[entityA] = destroyable;
    }
}
