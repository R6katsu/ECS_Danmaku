using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// ���g�̉�]�̏��
/// </summary>
[Serializable]
public struct RotationData : IComponentData
{
    [Header("��]����"), Tooltip("��]����")]
    public AxisType axisType;

    [Header("��]���x�i���̒l�͋t��]�j"), Tooltip("��]���x�i���̒l�͋t��]�j")]
    public float rotationSpeed;
}

/// <summary>
/// ���g�̉�]�̐ݒ�
/// </summary>
public class RotationAuthoring : MonoBehaviour
{
    [SerializeField, Header("���g�̉�]�̏��")]
    private RotationData _rotationData = new();

    public class Baker : Baker<RotationAuthoring>
    {
        public override void Bake(RotationAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            // ���g�̉�]�̏����A�^�b�`
            AddComponent(entity, src._rotationData);
        }
    }
}
