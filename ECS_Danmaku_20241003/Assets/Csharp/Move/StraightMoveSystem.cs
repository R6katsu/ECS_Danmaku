using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;

public partial struct StraightMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        foreach (var (straightMove, directionTravel, localTfm) in
                 SystemAPI.Query<RefRO<StraightMoveData>,
                                 RefRO<DirectionTravelData>,
                                 RefRW<LocalTransform>>())
        {
            // êiçsï˚å¸Ç…à⁄ìÆÇ∑ÇÈ
            float3 direction = DirectionTravelTypeConverter.GetDirectionVector(directionTravel.ValueRO.directionTravelType);
            localTfm.ValueRW.Position += direction * straightMove.ValueRO.moveParam.Speed * delta;
        }
    }
}
