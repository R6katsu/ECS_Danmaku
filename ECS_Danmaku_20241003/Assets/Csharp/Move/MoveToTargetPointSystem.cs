using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// �ڕW�ʒu�܂ňړ����鏈��
/// </summary>
public partial struct MoveToTargetPointSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;
        var entityManager = state.EntityManager;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // �ڕW�ʒu�܂ňړ����鏈��
        foreach (var (moveToTargetPoint, localTfm, entity) in
            SystemAPI.Query<RefRO<MoveToTargetPointData>,
                                 RefRW<LocalTransform>>()
                                 .WithEntityAccess())
        {
            // ���݈ʒu���擾
            float3 currentPosition = localTfm.ValueRW.Position;

            // �ڕW�ʒu���擾
            float3 targetPosition = moveToTargetPoint.ValueRO.targetPoint;

            // �ړ����x���擾
            float moveSpeed = moveToTargetPoint.ValueRO.moveParam.Speed;

            // �ړ��������v�Z
            float3 direction = math.normalize(targetPosition - currentPosition);

            // �t���[�����ƂɈړ��������v�Z
            float deltaMove = moveSpeed * delta;

            // �V�����ʒu���v�Z
            float3 newPosition = currentPosition + direction * deltaMove;

            // �ڕW�n�_�𒴂���
            if (math.distance(newPosition, targetPosition) < deltaMove)
            {
                // �K�v�Ȃ��Ȃ���MoveToTargetPointData���폜
                ecb.RemoveComponent<MoveToTargetPointData>(entity);
                continue;
            }

            // ���݈ʒu���X�V
            localTfm.ValueRW.Position = newPosition;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
