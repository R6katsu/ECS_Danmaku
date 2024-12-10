using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static BulletHelper;
using Unity.Mathematics;
using Unity.Collections;

#if UNITY_EDITOR
#endif

/// <summary>
/// ���g�̉�]�̏���
/// </summary>
[BurstCompile]
public partial struct RotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // EntityCommandBuffer���쐬
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

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
