using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static BulletHelper;
using Unity.Mathematics;

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
        foreach (var (rotationData, localTfm) in
             SystemAPI.Query
                 <RefRO<RotationData>,
                 RefRW<LocalTransform>>())
        {
            var axisType = rotationData.ValueRO.axisType;
            var rotationSpeed = rotationData.ValueRO.rotationSpeed;
            var rotation = localTfm.ValueRW.Rotation;

            // ��]�ʂ��v�Z
            var rotationAmount = AxisTypeHelper.GetAxisDirection(axisType) * rotationSpeed;

            // ��]�𔽉f
            localTfm.ValueRW.Rotation = math.mul(rotation, quaternion.Euler(math.radians(rotationAmount)));
        }
    }
}
