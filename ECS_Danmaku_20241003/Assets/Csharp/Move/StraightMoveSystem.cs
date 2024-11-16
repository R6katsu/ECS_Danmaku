using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;
using Unity.Burst;

/// <summary>
/// ���i�ړ��̏���
/// </summary>
[BurstCompile]
public partial struct StraightMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // ���i�ړ��̏���
        foreach (var (straightMove, localTfm) in
                 SystemAPI.Query<RefRO<StraightMoveData>,
                                 RefRW<LocalTransform>>())
        {
            // �i�s�����Ɉړ�����
            float3 direction = AxisTypeHelper.GetAxisDirection(straightMove.ValueRO.axisType);
            localTfm.ValueRW.Position += direction * straightMove.ValueRO.moveParam.Speed * delta;
        }
    }
}
