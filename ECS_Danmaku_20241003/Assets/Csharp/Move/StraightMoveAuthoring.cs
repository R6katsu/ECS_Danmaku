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

    /// <summary>
    /// ���i�ړ��̏��
    /// </summary>
    /// <param name="moveParam">�ړ��̐ݒ�l</param>
    public StraightMoveData(MoveParameter moveParam)
    {
        this.moveParam = moveParam;
    }
}

/// <summary>
/// ���i�ړ��̐ݒ�
/// </summary>
public class StraightMoveAuthoring : MonoBehaviour
{
    // �͈͂���o��ƍ폜�����悤�ɂ���
    // AABB�Ƃ�float3�Ƃ���������Transform�H

    [SerializeField]
    private MoveParameter _moveParam = new();

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var straightMove = new StraightMoveData(src._moveParam);

            // Data���A�^�b�`
            AddComponent(entity, straightMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
