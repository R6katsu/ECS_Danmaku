using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;

/// <summary>
/// �ΏۂɌ������Ē����ړ�����ׂ̏��
/// </summary>
public struct HomingMoveData : IComponentData
{
    public readonly MoveParameter moveParam;
    public readonly CampsType campsType;
    public Entity targetEntity;

    public HomingMoveData(MoveParameter moveParam, CampsType campsType, Entity targetPoint = new Entity())
    {
        this.moveParam = moveParam;
        this.campsType = campsType;
        this.targetEntity = targetPoint;
    }
}

public class HomingMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField]
    private CampsType _campsType = 0;

    /// <summary>
    /// �ړ��̐ݒ�l
    /// </summary>
    public MoveParameter MyMoveParam => _moveParam;

    /// <summary>
    /// �w�c�̎��
    /// </summary>
    public CampsType CampsType => _campsType;

    public class Baker : Baker<HomingMoveAuthoring>
    {
        public override void Bake(HomingMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // �ΏۂɌ������Ē����ړ�����ׂ̏����쐬
            var homingMove = new HomingMoveData(src.MyMoveParam, src.CampsType);

            // Data���A�^�b�`
            AddComponent(entity, homingMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
