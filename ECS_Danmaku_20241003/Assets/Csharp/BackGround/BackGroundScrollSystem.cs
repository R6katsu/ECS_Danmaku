using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �w�i�X�N���[��
/// </summary>
[BurstCompile]
public partial struct BackGroundScrollSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // �w�i���X�N���[��
        foreach (var (backGroundScroll, localTfm) in
                 SystemAPI.Query
                 <RefRO<BackGroundScrollData>,
                 RefRW<LocalTransform>>())
        {
            // ValueRO�ϐ������[�J���ϐ��ɑ��
            var initialPosition = backGroundScroll.ValueRO.initialPosition;
            var startPosition = backGroundScroll.ValueRO.startPosition;
            var endLength = backGroundScroll.ValueRO.endLength;
            var axisType = backGroundScroll.ValueRO.axisType;

            // ���̒l�� 1���`
            var positiveOne = 1.0f;

            // ����̃t���[���ł̈ړ���
            var frameMovement = Time.deltaTime * backGroundScroll.ValueRO.moveDirection;

            // �ڕW�ɓ��B������
            bool hasReachedTarget = false;

            // �ړ����������]���Ă��邩
            bool isMovingBackwards = math.length(startPosition) > 0.0f;

            // ���]���Ă����畄���𔽓]������
            endLength *= (isMovingBackwards) ? -positiveOne : positiveOne;

            // AxisType�ɑΉ�����e�������擾
            var initialPositionValue = AxisTypeHelper.GetAxisValue(axisType, initialPosition);
            var localPositionValue = AxisTypeHelper.GetAxisValue(axisType, localTfm.ValueRO.Position);
            var frameMovementValue = AxisTypeHelper.GetAxisValue(axisType, frameMovement);

            // �ړ������ɂ���ďI�������ƌ��݂̈ړ�������ς���
            float end = (isMovingBackwards) ? initialPositionValue + endLength : localPositionValue + frameMovementValue;
            float current = (isMovingBackwards) ? localPositionValue + frameMovementValue : initialPositionValue + endLength;

            // �ڕW�ɓ��B������
            hasReachedTarget = end >= current;

            // �ڕW�ɓ��B����
            if (hasReachedTarget)
            {
                // �ړ��J�n�ʒu�Ɉړ�
                localTfm.ValueRW.Position = startPosition;
            }
            else
            {
                // �ړ��𔽉f
                localTfm.ValueRW.Position += frameMovement;
            }
        }
    }
}
