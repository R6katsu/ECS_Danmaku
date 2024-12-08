using Unity.Burst;
using Unity.Entities;
using static DanmakuJobs;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endif

// リファクタリング済み

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
        var delta = SystemAPI.Time.DeltaTime;       // フレーム秒
        var elapsed = SystemAPI.Time.ElapsedTime;   // 経過時間

        // EndSimulationEntityCommandBufferSystemで最後に反映
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        // nWay弾の処理
        var N_WayJobHandle = new N_WayJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // 終了まで待機
        N_WayJobHandle.Complete();

        // タップ撃ちの処理
        var TapShootingJobHandle = new TapShootingJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            elapsedTime = elapsed
        }.ScheduleParallel(N_WayJobHandle);

        // 終了まで待機
        TapShootingJobHandle.Complete();

        // 結果を追加
        ecbSystem.AddJobHandleForProducer(TapShootingJobHandle);
    }
}