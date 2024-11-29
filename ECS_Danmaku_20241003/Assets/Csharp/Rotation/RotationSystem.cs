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
/// ©g‚Ì‰ñ“]‚Ìˆ—
/// </summary>
[BurstCompile]
public partial struct RotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // EntityCommandBuffer‚ğì¬
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        foreach (var (rotationData, localTfm, entity) in
             SystemAPI.Query
                 <RefRO<RotationData>,
                 RefRW<LocalTransform>>()
                 .WithEntityAccess())
        {
            if (rotationData.ValueRO.IsDataDeletion)
            {
                ecb.RemoveComponent<RotationData>(entity);
            }

            var axisType = rotationData.ValueRO.axisType;
            var rotationSpeed = rotationData.ValueRO.rotationSpeed;
            var rotation = localTfm.ValueRW.Rotation;

            // ‰ñ“]—Ê‚ğŒvZ
            var rotationAmount = AxisTypeHelper.GetAxisDirection(axisType) * rotationSpeed;

            // ‰ñ“]‚ğ”½‰f
            localTfm.ValueRW.Rotation = math.mul(rotation, quaternion.Euler(math.radians(rotationAmount)));
        }

        // EntityCommandBuffer‚ğÄ¶
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
