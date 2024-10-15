using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;
using static TriggerJobs;
using static PlayerHelper;
using static BulletHelper;

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
        var healthPointLookup = state.GetComponentLookup<PlayerHealthPointData>(false);
        var dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);

        // PL�ɒe�������������̏������Ăяo��
        var triggerJob = new PlayerDamageTriggerJob()
        {
            healthPointLookup = healthPointLookup,
            dealDamageLookup = dealDamageLookup,
            currentTime = currentTime
        };

        // �O�̃W���u����������
        state.Dependency.Complete();

        // �V�����W���u���X�P�W���[�����A���̃n���h���� state.Dependency �ɐݒ�
        var jobHandle = triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = jobHandle;
    }
}