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
        /*
        // EntityCommandBufferを作成
        var ecbTemp = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // 削除処理（foreach でエンティティをチェックして削除）
        foreach (var (nWay_DanmakuData, entity) in
            SystemAPI.Query<RefRO<N_Way_DanmakuData>>()
            .WithEntityAccess())
        {
            if (nWay_DanmakuData.ValueRO.IsDataDeletion)
            {
                ecbTemp.RemoveComponent<N_Way_DanmakuData>(entity);
            }
        }

        foreach (var (tapShooting_DanmakuData, entity) in
            SystemAPI.Query<RefRO<TapShooting_DanmakuData>>()
            .WithEntityAccess())
        {
            if (tapShooting_DanmakuData.ValueRO.IsDataDeletion)
            {
                ecbTemp.RemoveComponent<TapShooting_DanmakuData>(entity);
            }
        }

        // EntityCommandBufferを再生
        ecbTemp.Playback(state.EntityManager);
        ecbTemp.Dispose();
        */



        var delta = SystemAPI.Time.DeltaTime;
        var elapsed = SystemAPI.Time.ElapsedTime;

        // EndSimulationEntityCommandBufferSystem から EntityCommandBuffer を取得
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        // ジョブのスケジュール（弾幕生成処理）
        var N_WayJobHandle = new N_WayJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            deltaTime = delta
        }.ScheduleParallel(state.Dependency); // N_WayJobHandle は state.Dependency を使用して依存関係を管理

        N_WayJobHandle.Complete();

        // 次のジョブのスケジュール（TapShooting）
        var TapShootingJobHandle = new TapShootingJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            elapsedTime = elapsed
        }.ScheduleParallel(N_WayJobHandle); // N_WayJobHandle が依存関係として渡される

        TapShootingJobHandle.Complete();

        // EndSimulationEntityCommandBufferSystem にジョブ依存を追加
        ecbSystem.AddJobHandleForProducer(TapShootingJobHandle);
    }
}