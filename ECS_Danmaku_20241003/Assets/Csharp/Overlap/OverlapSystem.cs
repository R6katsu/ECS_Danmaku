using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static DanmakuJobs;
using static OverlapJobs;

public partial struct OverlapSystem : ISystem
{
    // 重なっていたらActionを実行する
    // 条件文などはAction内に容れる？　いや、相手が何のTagを持っているか分からないかも...

    // SphereOverlapData

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

        // Jobで実装して弾幕処理を呼び出す
        var jobHandle = new SphereOverlapJob
        {
            commandBuffer = ecb,
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // ecbを終える
        ecbSystem.AddJobHandleForProducer(jobHandle);
        state.Dependency = jobHandle;
    }
}
