using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static EntityCampsHelper;
using static EntityCategoryHelper;
using static MoveHelper;

/// <summary>
/// �ΏۂɌ������Ē����ړ�����ׂ̏��
/// </summary>
public struct HomingMoveData : IComponentData
{
    public readonly MoveParameter moveParam;

    [Tooltip("�ڕW�̐w�c")]
    public readonly EntityCampsType targetCampsType;

    [Tooltip("�ڕW�̃J�e�S��")]
    public readonly EntityCategory targetEntityCategory;

    public Entity targetEntity;

    /// <summary>
    /// �ΏۂɌ������Ē����ړ�����ׂ̏��
    /// </summary>
    /// <param name="moveParam">�ړ��̐ݒ�l</param>
    /// <param name="targetCampsType">�ڕW�̐w�c</param>
    /// <param name="targetEntityCategory">�ڕW�̃J�e�S��</param>
    /// <param name="targetEntity">�ڕW��Entity</param>
    public HomingMoveData(MoveParameter moveParam,�@EntityCampsType targetCampsType, EntityCategory targetEntityCategory, Entity targetEntity = new Entity())
    {
        this.moveParam = moveParam;
        this.targetCampsType = targetCampsType;
        this.targetEntityCategory = targetEntityCategory;
        this.targetEntity = targetEntity;
    }
}

public class HomingMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField, Header("�ڕW�̐w�c")]
    private EntityCampsType _targetCampsType = 0;

    [SerializeField, Header("�ڕW�̃J�e�S��")]
    private EntityCategory _targetEntityCategory = 0;

    /// <summary>
    /// �ړ��̐ݒ�l
    /// </summary>
    public MoveParameter MyMoveParam => _moveParam;

    /// <summary>
    /// �ڕW�̐w�c
    /// </summary>
    public EntityCampsType TargetCampsType => _targetCampsType;

    /// <summary>
    /// �ڕW�̃J�e�S��
    /// </summary>
    public EntityCategory TargetEntityCategory => _targetEntityCategory;

    public class Baker : Baker<HomingMoveAuthoring>
    {
        public override void Bake(HomingMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // �ΏۂɌ������Ē����ړ�����ׂ̏����쐬
            var homingMove = new HomingMoveData(src.MyMoveParam, src.TargetCampsType, src.TargetEntityCategory);

            // Data���A�^�b�`
            AddComponent(entity, homingMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
