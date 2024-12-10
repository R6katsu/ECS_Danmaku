using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// 向いている方向に移動する処理
/// </summary>
[BurstCompile]
public partial struct FacingDirectionMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // 向いている方向に移動する処理
        foreach (var (facingDirection, localTfm) in
                 SystemAPI.Query<RefRO<FacingDirectionMoveData>,
                                 RefRW<LocalTransform>>())
        {
            // ローカル座標系を使い、向いている方向への移動を計算
            float3 forward = math.forward(localTfm.ValueRO.Rotation);

            // 移動を反映
            localTfm.ValueRW.Position += forward * facingDirection.ValueRO.MoveParameter.Speed * delta;
        }
    }
}
