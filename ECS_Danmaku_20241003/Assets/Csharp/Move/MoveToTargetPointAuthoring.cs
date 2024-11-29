using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// �ڕW�ʒu�܂ňړ�����ׂ̏��
/// </summary>
public struct MoveToTargetPointData : IComponentData
{
    [Tooltip("�ړ��̐ݒ�l")]
    public readonly MoveParameter moveParam;

    [Tooltip("�ڕW�ʒu")]
    public readonly float3 targetPoint;

    /// <summary>
    /// �ڕW�ʒu�܂ňړ�����ׂ̏��
    /// </summary>
    /// <param name="moveParam">�ړ��̐ݒ�l</param>
    /// <param name="targetPoint">�ڕW�ʒu</param>
    public MoveToTargetPointData(MoveParameter moveParam, float3 targetPoint)
    {
        this.moveParam = moveParam;
        this.targetPoint = targetPoint;
    }
}

/// <summary>
/// �ڕW�ʒu�܂ňړ�����ׂ̐ݒ�
/// </summary>
public class MoveToTargetPointAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField, Header("�ڕW�ʒu")]
    private float3 _targetPoint = float3.zero;

    public class Baker : Baker<MoveToTargetPointAuthoring>
    {
        public override void Bake(MoveToTargetPointAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var moveToTargetPointData = new MoveToTargetPointData
                (
                    src._moveParam,
                    src._targetPoint
                );

            var moveToTargetPoint = moveToTargetPointData;

            // Data���A�^�b�`
            AddComponent(entity, moveToTargetPoint);
            AddComponent(entity, new MoveTag());
        }
    }
}
