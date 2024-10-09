using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FacingDirectionMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        foreach (var (facingDirection, localTfm) in
                 SystemAPI.Query<RefRO<FacingDirectionMoveData>,
                                 RefRW<LocalTransform>>())
        {
            // ローカル座標系を使い、向いている方向への移動を計算
            float3 forward = math.forward(localTfm.ValueRO.Rotation);
            localTfm.ValueRW.Position += forward * facingDirection.ValueRO.moveParam.Speed * delta;
        }
    }
}
