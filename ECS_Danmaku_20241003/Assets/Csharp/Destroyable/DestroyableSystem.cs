using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;

/// <summary>
/// 削除処理
/// </summary>
[BurstCompile]
public partial struct DestroyableSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // ecbを取得
        var ecbSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // 削除処理を呼び出す
        var job = new DestroyJob
        {
            ecb = ecb
        };

        // ジョブをスケジュール
        var handle = job.ScheduleParallel(state.Dependency);

        // 依存関係を伝える
        ecbSystem.AddJobHandleForProducer(handle);

        // 次のシステム更新前に依存関係を更新
        state.Dependency = handle;
    }
}

/// <summary>
/// 削除処理
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
