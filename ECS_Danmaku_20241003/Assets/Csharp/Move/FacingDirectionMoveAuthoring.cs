using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
using static BulletHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;
#endif

/// <summary>
/// �����Ă�������Ɉړ�����ׂ̏��
/// </summary>
public struct FacingDirectionMoveData : IComponentData
{
    public readonly MoveParameter moveParam;

    public FacingDirectionMoveData(MoveParameter moveParam)
    {
        this.moveParam = moveParam;
    }
}

/// <summary>
/// �����Ă�������Ɉړ�����ׂ̐ݒ�
/// </summary>
public class FacingDirectionMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    public class FacingDirectionMoveBaker : Baker<FacingDirectionMoveAuthoring>
    {
        public override void Bake(FacingDirectionMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // �����Ă�������Ɉړ�����ׂ̏����쐬
            var facingDirectionMove = new FacingDirectionMoveData(src._moveParam);

            // Data���A�^�b�`
            AddComponent(entity, facingDirectionMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
