using Unity.Burst;
using Unity.Entities;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// Entity�̍폜����
/// </summary>
[BurstCompile]
public partial struct DestroyableSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // EndSimulationEntityCommandBufferSystem�ōŌ�ɔ��f
        var ecbSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // Entity�̍폜����
        var job = new DestroyJob
        {
            ecb = ecb
        };
        var handle = job.ScheduleParallel(state.Dependency);

        // �ˑ��֌W��ǉ�
        ecbSystem.AddJobHandleForProducer(handle);

        // �ˑ��֌W���X�V
        state.Dependency = handle;
    }
}

/// <summary>
/// Entity�̍폜����
/// </summary>
[BurstCompile]
partial struct DestroyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(
        Entity entity,
        DestroyableData destroyableData,
        [EntityIndexInQuery] int index)
    {
        // �t���O�������Ă�����Entity���폜
        if (destroyableData.isKilled)
        {
            ecb.DestroyEntity(index, entity);
        }
    }
}
