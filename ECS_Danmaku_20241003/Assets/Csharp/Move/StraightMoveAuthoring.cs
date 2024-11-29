using Unity.Entities;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static BulletHelper;
using static UnityEngine.EventSystems.EventTrigger;
#endif

/// <summary>
/// ���i�ړ��̏��
/// </summary>
public struct StraightMoveData : IComponentData
{
    [Tooltip("�ړ��̐ݒ�l")]
    public readonly MoveParameter moveParam;

    [Tooltip("�i�s����")]
    public readonly AxisType axisType;

    /// <summary>
    /// ���i�ړ��̏��
    /// </summary>
    /// <param name="moveParam">�ړ��̐ݒ�l</param>
    /// <param name="axisType">�i�s����</param>
    public StraightMoveData(MoveParameter moveParam, AxisType axisType)
    {
        this.moveParam = moveParam;
        this.axisType = axisType;
    }
}

/// <summary>
/// ���i�ړ��̐ݒ�
/// </summary>
public class StraightMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField, Header("�i�s����")]
    private AxisType _axisType = 0;

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var straightMoveData = new StraightMoveData
                (
                    src._moveParam, 
                    src._axisType
                );

            var straightMove = straightMoveData;

            // Data���A�^�b�`
            AddComponent(entity, straightMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
