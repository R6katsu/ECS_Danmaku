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
        /*
        // EntityCommandBuffer���쐬
        var ecbTemp = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // �폜�����iforeach �ŃG���e�B�e�B���`�F�b�N���č폜�j
        foreach (var (nWay_DanmakuData, entity) in
            SystemAPI.Query<RefRO<N_Way_DanmakuData>>()
            .WithEntityAccess())
        {
            if (nWay_DanmakuData.ValueRO.IsDataDeletion)
            {
                ecbTemp.RemoveComponent<N_Way_DanmakuData>(entity);
            }
        }

        foreach (var (tapShooting_DanmakuData, entity) in
            SystemAPI.Query<RefRO<TapShooting_DanmakuData>>()
            .WithEntityAccess())
        {
            if (tapShooting_DanmakuData.ValueRO.IsDataDeletion)
            {
                ecbTemp.RemoveComponent<TapShooting_DanmakuData>(entity);
            }
        }

        // EntityCommandBuffer���Đ�
        ecbTemp.Playback(state.EntityManager);
        ecbTemp.Dispose();
        */



        var delta = SystemAPI.Time.DeltaTime;
        var elapsed = SystemAPI.Time.ElapsedTime;

        // EndSimulationEntityCommandBufferSystem ���� EntityCommandBuffer ���擾
        var ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        // �W���u�̃X�P�W���[���i�e�����������j
        var N_WayJobHandle = new N_WayJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            deltaTime = delta
        }.ScheduleParallel(state.Dependency); // N_WayJobHandle �� state.Dependency ���g�p���Ĉˑ��֌W���Ǘ�

        N_WayJobHandle.Complete();

        // ���̃W���u�̃X�P�W���[���iTapShooting�j
        var TapShootingJobHandle = new TapShootingJob
        {
            commandBuffer = ecb.AsParallelWriter(),
            elapsedTime = elapsed
        }.ScheduleParallel(N_WayJobHandle); // N_WayJobHandle ���ˑ��֌W�Ƃ��ēn�����

        TapShootingJobHandle.Complete();

        // EndSimulationEntityCommandBufferSystem �ɃW���u�ˑ���ǉ�
        ecbSystem.AddJobHandleForProducer(TapShootingJobHandle);
    }
}