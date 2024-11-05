using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;
using static TriggerJobs;
using static PlayerHelper;
using static BulletHelper;
using static EnemyHelper;
using static HealthPointDatas;
using Unity.Transforms;

/// <summary>
/// 接触した際の処理を呼び出す
/// </summary>
[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    private ComponentLookup<PlayerHealthPointData> _playerHealthPointLookup;
    private ComponentLookup<EnemyHealthPointData> _enemyHealthPointLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;
    private ComponentLookup<RemainingPierceCountData> _remainingPierceCountLookup;
    private ComponentLookup<LocalTransform> _localTransformLookup;
    private ComponentLookup<VFXCreationData> _vfxCreationLookup;
    private ComponentLookup<AudioPlayData> _audioPlayLookup;

    public void OnCreate(ref SystemState state)
    {
        // 取得する
        _playerHealthPointLookup = state.GetComponentLookup<PlayerHealthPointData>(false);
        _enemyHealthPointLookup = state.GetComponentLookup<EnemyHealthPointData>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
        _remainingPierceCountLookup = state.GetComponentLookup<RemainingPierceCountData>(false);
        _localTransformLookup = state.GetComponentLookup<LocalTransform>(false);
        _vfxCreationLookup = state.GetComponentLookup<VFXCreationData>(false);
        _audioPlayLookup = state.GetComponentLookup<AudioPlayData>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        var currentTime = SystemAPI.Time.ElapsedTime;

        // 更新する
        _playerHealthPointLookup.Update(ref state);
        _enemyHealthPointLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);
        _remainingPierceCountLookup.Update(ref state);
        _localTransformLookup.Update(ref state);
        _vfxCreationLookup.Update(ref state);
        _audioPlayLookup.Update(ref state);

        // PLに弾が当たった時の処理を呼び出す
        var playerDamage = new PlayerDamageTriggerJob()
        {
            healthPointLookup = _playerHealthPointLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            remainingPierceCountLookup = _remainingPierceCountLookup,
            localTransformLookup = _localTransformLookup,
            vfxCreationLookup = _vfxCreationLookup,
            audioPlayLookup = _audioPlayLookup,
            currentTime = currentTime
        };

        // 前のジョブを完了する
        state.Dependency.Complete();    // 並列処理の利点が失われる

        var playerJobHandle = playerDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = playerJobHandle;

        // 敵に弾が当たった時の処理を呼び出す
        var enemyDamage = new EnemyDamageTriggerJob()
        {
            healthPointLookup = _enemyHealthPointLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            remainingPierceCountLookup = _remainingPierceCountLookup,
            localTransformLookup = _localTransformLookup,
            vfxCreationLookup = _vfxCreationLookup,
            audioPlayLookup = _audioPlayLookup
        };

        // 前のジョブを完了する
        state.Dependency.Complete();

        var enemyJobHandle = enemyDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // ジョブの依存関係を更新する
        state.Dependency = enemyJobHandle;
    }
}