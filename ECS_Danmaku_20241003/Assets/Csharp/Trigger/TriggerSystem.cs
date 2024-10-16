using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;
using static TriggerJobs;
using static PlayerHelper;
using static BulletHelper;
using static EnemyHelper;

/// <summary>
/// 接触した際の処理を呼び出す
/// </summary>
[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var currentTime = SystemAPI.Time.ElapsedTime;

        // インスタンスを取得する為の変数
        var playerHealthPointLookup = state.GetComponentLookup<PlayerHealthPointData>(false);
        var enemyHealthPointLookup = state.GetComponentLookup<EnemyHealthPointData>(false);
        var dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        var destroyableLookup = state.GetComponentLookup<DestroyableData>(false);

        // PLに弾が当たった時の処理を呼び出す
        var playerDamage = new PlayerDamageTriggerJob()
        {
            healthPointLookup = playerHealthPointLookup,
            dealDamageLookup = dealDamageLookup,
            destroyableLookup = destroyableLookup,
            currentTime = currentTime
        };

        // 前のジョブを完了する
        state.Dependency.Complete();

        var playerJobHandle = playerDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = playerJobHandle;

        // 敵に弾が当たった時の処理を呼び出す
        var enemyDamage = new EnemyDamageTriggerJob()   // エラーは出なくなったが以前敵にダメージを与えられない
        {
            healthPointLookup = enemyHealthPointLookup,
            dealDamageLookup = dealDamageLookup,
            destroyableLookup = destroyableLookup
        };

        // 前のジョブを完了する
        state.Dependency.Complete();

        var enemyJobHandle = enemyDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = enemyJobHandle;
    }
}