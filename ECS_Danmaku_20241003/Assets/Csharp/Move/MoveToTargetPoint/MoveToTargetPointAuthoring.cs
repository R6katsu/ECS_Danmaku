using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;
using System;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �ڕW�ʒu�܂ňړ�����ׂ̏��
/// </summary>
[Serializable]
public struct MoveToTargetPointData : IComponentData
{
    [SerializeField, Header("�ړ��̐ݒ�l")]
    private MoveParameter _moveParameter;

    [SerializeField, Header("�ڕW�ʒu")]
    private float3 _targetPoint;

    /// <summary>
    /// �ړ��̐ݒ�l
    /// </summary>
    public MoveParameter MoveParameter => _moveParameter;

    /// <summary>
    /// �ڕW�ʒu
    /// </summary>
    public float3 TargetPoint => _targetPoint;

    /// <summary>
    /// �ڕW�ʒu�܂ňړ�����ׂ̏��
    /// </summary>
    /// <param name="moveParameter">�ړ��̐ݒ�l</param>
    /// <param name="targetPoint">�ڕW�ʒu</param>
    public MoveToTargetPointData(MoveParameter moveParameter, float3 targetPoint)
    {
        _moveParameter = moveParameter;
        _targetPoint = targetPoint;
    }
}

/// <summary>
/// �ڕW�ʒu�܂ňړ�����ׂ̐ݒ�
/// </summary>
public class MoveToTargetPointAuthoring : MonoBehaviour
{
    [SerializeField, Header("�ڕW�ʒu�܂ňړ�����ׂ̏��")]
    private MoveToTargetPointData _moveToTargetPointData = new();

    public class Baker : Baker<MoveToTargetPointAuthoring>
    {
        public override void Bake(MoveToTargetPointAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Data���A�^�b�`
            AddComponent(entity, src._moveToTargetPointData);
            AddComponent(entity, new MoveTag());
        }
    }
}
