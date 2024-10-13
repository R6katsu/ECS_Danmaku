using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;
using System.Collections.Generic;
using static TriggerJobs;
using static PlayerHelper;
using static HealthHelper;
using static PlayerAuthoring;
using Unity.Collections;
using static UnityEngine.InputSystem.HID.HID;
using static DanmakuJobs;
using static EnemyHelper;
using static BulletHelper;

/// <summary>
/// 接触した際の処理を呼び出す
/// </summary>
[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // PLに弾が当たった時の処理を呼び出す
        var triggerJob = new PlayerDamageTriggerJob()
        {
            healthPointLookup = state.GetComponentLookup<PlayerHealthPointData>(false),
            dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false)
        };

        // 前のジョブを完了する
        state.Dependency.Complete();

        // 新しいジョブをスケジュールし、そのハンドルを state.Dependency に設定
        var jobHandle = triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = jobHandle;
    }
}