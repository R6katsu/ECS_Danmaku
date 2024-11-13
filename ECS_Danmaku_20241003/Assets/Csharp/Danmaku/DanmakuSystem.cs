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
    // 弾幕を放つのは敵やPL自身ではない
    // 敵やPLの子オブジェクトとして追従している銃口から放たれる
    // 自身の子を取得し、その弾幕を取得する
    // というか、敵やPL自身が持てる銃口の数に制限を設ける
    // そしてその位置に子オブジェクトを動的に作成する
    // 子オブジェクトの銃口に渡すパラメーターはPLのインスペクタから設定する？
    // 



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
