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
/// �ڐG�����ۂ̏������Ăяo��
/// </summary>
[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // PL�ɒe�������������̏������Ăяo��
        var triggerJob = new PlayerDamageTriggerJob()
        {
            healthPointLookup = state.GetComponentLookup<PlayerHealthPointData>(false),
            dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false)
        };

        // �O�̃W���u����������
        state.Dependency.Complete();

        // �V�����W���u���X�P�W���[�����A���̃n���h���� state.Dependency �ɐݒ�
        var jobHandle = triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = jobHandle;
    }
}