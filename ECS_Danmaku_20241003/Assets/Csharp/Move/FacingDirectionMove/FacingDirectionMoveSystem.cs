using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �����Ă�������Ɉړ����鏈��
/// </summary>
[BurstCompile]
public partial struct FacingDirectionMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        // �����Ă�������Ɉړ����鏈��
        foreach (var (facingDirection, localTfm) in
                 SystemAPI.Query<RefRO<FacingDirectionMoveData>,
                                 RefRW<LocalTransform>>())
        {
            // ���[�J�����W�n���g���A�����Ă�������ւ̈ړ����v�Z
            float3 forward = math.forward(localTfm.ValueRO.Rotation);

            // �ړ��𔽉f
            localTfm.ValueRW.Position += forward * facingDirection.ValueRO.MoveParameter.Speed * delta;
        }
    }
}
