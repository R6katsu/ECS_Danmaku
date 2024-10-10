using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static DanmakuJobs;
using static OverlapJobs;

public partial struct OverlapSystem : ISystem
{
    // �d�Ȃ��Ă�����Action�����s����
    // �������Ȃǂ�Action���ɗe���H�@����A���肪����Tag�������Ă��邩������Ȃ�����...

    // SphereOverlapData

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

        // Job�Ŏ������Ēe���������Ăяo��
        var jobHandle = new SphereOverlapJob
        {
            commandBuffer = ecb,
            deltaTime = delta
        }.ScheduleParallel(state.Dependency);

        // ecb���I����
        ecbSystem.AddJobHandleForProducer(jobHandle);
        state.Dependency = jobHandle;
    }
}
