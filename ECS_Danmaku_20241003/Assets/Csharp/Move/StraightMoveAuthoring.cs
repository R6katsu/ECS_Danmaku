using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

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

    /// <summary>
    /// �ړ��̐ݒ�l
    /// </summary>
    public MoveParameter MoveParam => _moveParam;

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var straightMove = new StraightMoveData(src.MoveParam);

            // Data���A�^�b�`
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), straightMove);
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new MoveTag());
        }
    }
}
