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
    [SerializeField, Header("��]����")]
    private AxisType _axisType;

    [SerializeField, Header("��]���x�i���̒l�͋t��]�j")]
    private float _rotationSpeed;

    /// <summary>
    /// ��]����
    /// </summary>
    public AxisType AxisType => _axisType;

    /// <summary>
    /// ��]���x�i���̒l�͋t��]�j
    /// </summary>
    public float RotationSpeed => _rotationSpeed;

    /// <summary>
    /// ���g�̉�]�̏��
    /// </summary>
    /// <param name="axisType">��]����</param>
    /// <param name="rotationSpeed">��]���x�i���̒l�͋t��]�j</param>
    public RotationData(AxisType axisType, float rotationSpeed)
    {
        _axisType = axisType;
        _rotationSpeed = rotationSpeed;
    }
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
