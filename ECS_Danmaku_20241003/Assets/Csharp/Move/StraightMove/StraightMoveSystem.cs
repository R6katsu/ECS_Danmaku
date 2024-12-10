using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveHelper;
#endif

// リファクタリング済み

/// <summary>
/// 直進移動の処理
/// </summary>
[BurstCompile]
public partial struct StraightMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // 直進移動の処理
        foreach (var (straightMove, localTfm) in
                 SystemAPI.Query<RefRO<StraightMoveData>,
                                 RefRW<LocalTransform>>())
        {
            // 進行方向に移動する
            float3 direction = AxisTypeHelper.GetAxisDirection(straightMove.ValueRO.AxisType);
            localTfm.ValueRW.Position += direction * straightMove.ValueRO.MoveParameter.Speed * delta;
        }
    }
}
