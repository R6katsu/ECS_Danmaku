using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;

using TriggerEvent = Unity.Physics.TriggerEvent;

[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var triggerJob = new TriggerJob();
        triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    public partial struct TriggerJob : ITriggerEventsJob
    {
        public void Execute(TriggerEvent triggerEvent)
        {
            Debug.Log($"A：{triggerEvent.EntityA}, B：{triggerEvent.EntityB}");
        }
    }

    /*
    public void OnCreate(ref SystemState state)
    {
        // 物理システムの依存関係を取得
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // 物理ワールドとシミュレーションシステムを取得
        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        // TriggerEventJobのスケジューリング
        var triggerJob = new TriggerEventJob
        {
            MoveComponentLookup = state.GetComponentLookup<PlayerData>(true)
        };

        // エンティティAのコンポーネントの有無を確認し、該当する場合のみジョブをスケジュール
        var triggerEvents = GetTriggerEvents(); // トリガーイベントのリストを取得するメソッドを実装
        foreach (var triggerEvent in triggerEvents)
        {
            // entityA が MoveComponent を持っているか確認
            if (triggerJob.MoveComponentLookup.HasComponent(triggerEvent.EntityA))
            {
                state.Dependency = triggerJob.Schedule(triggerEvent, simulation, state.Dependency);
            }
        }

        // TriggerEventJobのスケジューリング
        //var triggerJob = new TriggerEventJob
        //{
            // 必要なデータのセットアップ
        //};

        // ITriggerEventsJobExtensions.Scheduleを使用する
        //state.Dependency = triggerJob.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    private struct TriggerEventJob : ITriggerEventsJob
    {
        public ComponentLookup<PlayerData> MoveComponentLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            // Triggerされた時の処理
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            PlayerData moveComponent = MoveComponentLookup[entityA];
        }
    }
    */
}