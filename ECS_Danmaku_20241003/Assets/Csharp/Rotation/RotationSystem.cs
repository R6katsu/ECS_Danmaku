using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletHelper;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// ���g�̉�]�̏���
/// </summary>
[BurstCompile]
public partial struct RotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        // RotationData��L����Entity�ɉ�]�����s
        foreach (var (rotationData, localTfm, entity) in
             SystemAPI.Query
                 <RefRO<RotationData>,
                 RefRW<LocalTransform>>()
                 .WithEntityAccess())
        {
            var axisType = rotationData.ValueRO.AxisType;
            var rotationSpeed = rotationData.ValueRO.RotationSpeed;
            var rotation = localTfm.ValueRW.Rotation;

            // ��]�ʂ��v�Z
            var rotationAmount = AxisTypeHelper.GetAxisDirection(axisType) * rotationSpeed;

            // ��]�𔽉f
            localTfm.ValueRW.Rotation = math.mul(rotation, quaternion.Euler(math.radians(rotationAmount)));
        }

        // EntityCommandBuffer���Đ�
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
