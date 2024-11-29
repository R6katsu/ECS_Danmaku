using Unity.Burst;
using Unity.Entities;
using static DanmakuJobs;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endif

/// <summary>
/// 弾幕を生成する処理を呼び出す
/// </summary>
//[BurstCompile]
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
        var elapsed = SystemAPI.Time.ElapsedTime;

        // ecbの準備
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // n-Way弾を生成する処理を呼び出す
        var N_WayJobHandle = new N_WayJob
        {
            commandBuffer = ecb,
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // ecbを終える
        ecbSystem.AddJobHandleForProducer(N_WayJobHandle);

        // TapShootingJobをN_WayJobの後にスケジュール
        var TapShootingJobHandle = new TapShootingJob
        {
            commandBuffer = ecb,
            elapsedTime = elapsed
        }.ScheduleParallel(N_WayJobHandle);

        // ecbを終える
        ecbSystem.AddJobHandleForProducer(TapShootingJobHandle);

        // 最終的な依存関係を更新
        state.Dependency = TapShootingJobHandle;
    }
}
