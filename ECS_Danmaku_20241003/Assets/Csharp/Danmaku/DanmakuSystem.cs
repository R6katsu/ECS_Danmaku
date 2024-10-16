using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static DanmakuJobs;

/// <summary>
/// 弾幕を生成する処理を呼び出す
/// </summary>
[BurstCompile]
public partial struct DanmakuSystem : ISystem
{
    private SystemHandle _ecbSystemHandle;

    public void OnCreate(ref SystemState state)
    {
        // SystemHandleを取得
        _ecbSystemHandle = state.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // ecbの準備
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // 弾幕を生成する処理を呼び出す
        var jobHandle = new N_WayJob
        {
            commandBuffer = ecb,
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // ecbを終える
        ecbSystem.AddJobHandleForProducer(jobHandle);
        state.Dependency = jobHandle;
    }
}
