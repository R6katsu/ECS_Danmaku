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
/// �ڐG�����ۂ̏������Ăяo��
/// </summary>
[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    private ComponentLookup<PlayerHealthPointData> _playerHealthPointLookup;
    private ComponentLookup<EnemyHealthPointData> _enemyHealthPointLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;
    private ComponentLookup<RemainingPierceCountData> _remainingPierceCountLookup;

    public void OnCreate(ref SystemState state)
    {
        // �擾����
        _playerHealthPointLookup = state.GetComponentLookup<PlayerHealthPointData>(false);
        _enemyHealthPointLookup = state.GetComponentLookup<EnemyHealthPointData>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
        _remainingPierceCountLookup = state.GetComponentLookup<RemainingPierceCountData>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        var currentTime = SystemAPI.Time.ElapsedTime;

        // �X�V����
        _playerHealthPointLookup.Update(ref state);
        _enemyHealthPointLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);
        _remainingPierceCountLookup.Update(ref state);

        // PL�ɒe�������������̏������Ăяo��
        var playerDamage = new PlayerDamageTriggerJob()
        {
            healthPointLookup = _playerHealthPointLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            remainingPierceCountLookup = _remainingPierceCountLookup,
            currentTime = currentTime
        };

        // �O�̃W���u����������
        state.Dependency.Complete();    // ���񏈗��̗��_��������

        var playerJobHandle = playerDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = playerJobHandle;

        // �G�ɒe�������������̏������Ăяo��
        var enemyDamage = new EnemyDamageTriggerJob()
        {
            healthPointLookup = _enemyHealthPointLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            remainingPierceCountLookup = _remainingPierceCountLookup
        };

        // �O�̃W���u����������
        state.Dependency.Complete();

        var enemyJobHandle = enemyDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = enemyJobHandle;
    }
}