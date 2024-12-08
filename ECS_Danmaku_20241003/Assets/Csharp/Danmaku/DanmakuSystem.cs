using Unity.Burst;
using Unity.Entities;
using static DanmakuJobs;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �e���𐶐����鏈�����Ăяo��
/// </summary>
[BurstCompile]
public partial struct DanmakuSystem : ISystem
{
    private SystemHandle _ecbSystemHandle;

    public void OnCreate(ref SystemState state)
    {
        // SystemHandle���擾
        _ecbSystemHandle = state.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;       // �t���[���b
        var elapsed = SystemAPI.Time.ElapsedTime;   // �o�ߎ���

        // EndSimulationEntityCommandBufferSystem�ōŌ�ɔ��f
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        // nWay�e�̏���
        var N_WayJobHandle = new N_WayJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // �I���܂őҋ@
        N_WayJobHandle.Complete();

        // �^�b�v�����̏���
        var TapShootingJobHandle = new TapShootingJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            elapsedTime = elapsed
        }.ScheduleParallel(N_WayJobHandle);

        // �I���܂őҋ@
        TapShootingJobHandle.Complete();

        // ���ʂ�ǉ�
        ecbSystem.AddJobHandleForProducer(TapShootingJobHandle);
    }
}