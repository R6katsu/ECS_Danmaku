using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static DanmakuJobs;

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
        var delta = SystemAPI.Time.DeltaTime;

        // ecb�̏���
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // �e���𐶐����鏈�����Ăяo��
        var jobHandle = new N_WayJob
        {
            commandBuffer = ecb,
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // ecb���I����
        ecbSystem.AddJobHandleForProducer(jobHandle);
        state.Dependency = jobHandle;
    }
}
