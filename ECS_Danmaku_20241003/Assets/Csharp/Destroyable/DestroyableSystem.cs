using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;

/// <summary>
/// �폜����
/// </summary>
[BurstCompile]
public partial struct DestroyableSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // ecb���擾
        var ecbSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // �폜�������Ăяo��
        var job = new DestroyJob
        {
            ecb = ecb
        };

        // �W���u���X�P�W���[��
        var handle = job.ScheduleParallel(state.Dependency);

        // �ˑ��֌W��`����
        ecbSystem.AddJobHandleForProducer(handle);

        // ���̃V�X�e���X�V�O�Ɉˑ��֌W���X�V
        state.Dependency = handle;
    }
}

/// <summary>
/// �폜����
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
