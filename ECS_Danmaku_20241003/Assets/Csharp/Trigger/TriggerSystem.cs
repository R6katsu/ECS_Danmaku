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
    public void OnUpdate(ref SystemState state)
    {
        var currentTime = SystemAPI.Time.ElapsedTime;

        // �C���X�^���X���擾����ׂ̕ϐ�
        var playerHealthPointLookup = state.GetComponentLookup<PlayerHealthPointData>(false);
        var enemyHealthPointLookup = state.GetComponentLookup<EnemyHealthPointData>(false);
        var dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        var destroyableLookup = state.GetComponentLookup<DestroyableData>(false);

        // PL�ɒe�������������̏������Ăяo��
        var playerDamage = new PlayerDamageTriggerJob()
        {
            healthPointLookup = playerHealthPointLookup,
            dealDamageLookup = dealDamageLookup,
            destroyableLookup = destroyableLookup,
            currentTime = currentTime
        };

        // �O�̃W���u����������
        state.Dependency.Complete();

        var playerJobHandle = playerDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = playerJobHandle;

        // �G�ɒe�������������̏������Ăяo��
        var enemyDamage = new EnemyDamageTriggerJob()   // �G���[�͏o�Ȃ��Ȃ������ȑO�G�Ƀ_���[�W��^�����Ȃ�
        {
            healthPointLookup = enemyHealthPointLookup,
            dealDamageLookup = dealDamageLookup,
            destroyableLookup = destroyableLookup
        };

        // �O�̃W���u����������
        state.Dependency.Complete();

        var enemyJobHandle = enemyDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = enemyJobHandle;
    }
}