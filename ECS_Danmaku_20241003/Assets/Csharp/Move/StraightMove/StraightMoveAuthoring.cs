using Unity.Entities;
using System;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static BulletHelper;
using static UnityEngine.EventSystems.EventTrigger;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// ���i�ړ��̏��
/// </summary>
[Serializable]
public struct StraightMoveData : IComponentData
{
    [SerializeField, Header("�ړ��̐ݒ�l")]
    private MoveParameter _moveParam;

    [SerializeField, Header("�i�s����")]
    private AxisType _axisType;

    /// <summary>
    /// �ړ��̐ݒ�l
    /// </summary>
    public MoveParameter MoveParameter => _moveParam;

    /// <summary>
    /// �i�s����
    /// </summary>
    public AxisType AxisType => _axisType;

    /// <summary>
    /// ���i�ړ��̏��
    /// </summary>
    /// <param name="moveParam">�ړ��̐ݒ�l</param>
    /// <param name="axisType">�i�s����</param>
    public StraightMoveData(MoveParameter moveParam, AxisType axisType)
    {
        _moveParam = moveParam;
        _axisType = axisType;
    }
}

/// <summary>
/// ���i�ړ��̐ݒ�
/// </summary>
public class StraightMoveAuthoring : MonoBehaviour
{
    [SerializeField, Header("���i�ړ��̏��")]
    private StraightMoveData _straightMoveData = new();

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Data���A�^�b�`
            AddComponent(entity, src._straightMoveData);
            AddComponent(entity, new MoveTag());
        }
    }
}
