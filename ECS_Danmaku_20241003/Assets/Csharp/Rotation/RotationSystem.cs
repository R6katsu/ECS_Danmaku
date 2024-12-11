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

// リファクタリング済み

/// <summary>
/// 自身の回転の処理
/// </summary>
[BurstCompile]
public partial struct RotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        // RotationDataを有するEntityに回転を実行
        foreach (var (rotationData, localTfm, entity) in
             SystemAPI.Query
                 <RefRO<RotationData>,
                 RefRW<LocalTransform>>()
                 .WithEntityAccess())
        {
            var axisType = rotationData.ValueRO.AxisType;
            var rotationSpeed = rotationData.ValueRO.RotationSpeed;
            var rotation = localTfm.ValueRW.Rotation;

            // 回転量を計算
            var rotationAmount = AxisTypeHelper.GetAxisDirection(axisType) * rotationSpeed;

            // 回転を反映
            localTfm.ValueRW.Rotation = math.mul(rotation, quaternion.Euler(math.radians(rotationAmount)));
        }

        // EntityCommandBufferを再生
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
