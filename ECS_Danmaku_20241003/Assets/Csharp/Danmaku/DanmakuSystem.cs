using Unity.Burst;
using Unity.Entities;
using static DanmakuJobs;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endif

/// <summary>
/// �e���𐶐����鏈�����Ăяo��
/// </summary>
//[BurstCompile]
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
        var delta = SystemAPI.Time.DeltaTime;
        var elapsed = SystemAPI.Time.ElapsedTime;

        // ecb�̏���
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // n-Way�e�𐶐����鏈�����Ăяo��
        var N_WayJobHandle = new N_WayJob
        {
            commandBuffer = ecb,
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // ecb���I����
        ecbSystem.AddJobHandleForProducer(N_WayJobHandle);

        // TapShootingJob��N_WayJob�̌�ɃX�P�W���[��
        var TapShootingJobHandle = new TapShootingJob
        {
            commandBuffer = ecb,
            elapsedTime = elapsed
        }.ScheduleParallel(N_WayJobHandle);

        // ecb���I����
        ecbSystem.AddJobHandleForProducer(TapShootingJobHandle);

        // �ŏI�I�Ȉˑ��֌W���X�V
        state.Dependency = TapShootingJobHandle;
    }
}
