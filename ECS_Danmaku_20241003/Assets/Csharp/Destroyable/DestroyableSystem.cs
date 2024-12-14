using Unity.Burst;
using Unity.Entities;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endif

// リファクタリング済み

/// <summary>
/// Entityの削除処理
/// </summary>
[BurstCompile]
public partial struct DestroyableSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // EndSimulationEntityCommandBufferSystemで最後に反映
        var ecbSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // Entityの削除処理
        var job = new DestroyJob
        {
            ecb = ecb
        };
        var handle = job.ScheduleParallel(state.Dependency);

        // 依存関係を追加
        ecbSystem.AddJobHandleForProducer(handle);

        // 依存関係を更新
        state.Dependency = handle;
    }
}

/// <summary>
/// Entityの削除処理
/// </summary>
[BurstCompile]
partial struct DestroyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(
        Entity entity,
        DestroyableData destroyableData,
        [EntityIndexInQuery] int index)
    {
        // フラグが立っていたらEntityを削除
        if (destroyableData.isKilled)
        {
            ecb.DestroyEntity(index, entity);
        }
    }
}
