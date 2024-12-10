using Unity.Entities;
using UnityEngine;
using static MoveHelper;
using System;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static BulletHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �����Ă�������Ɉړ�����ׂ̏��
/// </summary>
[Serializable]
public struct FacingDirectionMoveData : IComponentData
{
    [SerializeField, Header("�ړ��ɕK�v�Ȑݒ�l")]
    private MoveParameter _moveParameter;

    /// <summary>
    /// �ړ��ɕK�v�Ȑݒ�l
    /// </summary>
    public MoveParameter MoveParameter => _moveParameter;

    /// <summary>
    /// �����Ă�������Ɉړ�����ׂ̏��
    /// </summary>
    /// <param name="moveParameter">�ړ��ɕK�v�Ȑݒ�l</param>
    public FacingDirectionMoveData(MoveParameter moveParameter)
    {
        _moveParameter = moveParameter;
    }
}

/// <summary>
/// �����Ă�������Ɉړ�����ׂ̐ݒ�
/// </summary>
public class FacingDirectionMoveAuthoring : MonoBehaviour
{
    [SerializeField, Header("�����Ă�������Ɉړ�����ׂ̏��")]
    private FacingDirectionMoveData _facingDirectionMoveData = new();

    public class FacingDirectionMoveBaker : Baker<FacingDirectionMoveAuthoring>
    {
        public override void Bake(FacingDirectionMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Data���A�^�b�`
            AddComponent(entity, src._facingDirectionMoveData);
            AddComponent(entity, new MoveTag());
        }
    }
}
