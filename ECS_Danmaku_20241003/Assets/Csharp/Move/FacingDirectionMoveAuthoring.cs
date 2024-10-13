using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static MoveHelper;

public struct FacingDirectionMoveData : IComponentData
{
    public readonly MoveParameter moveParam;

    public FacingDirectionMoveData(MoveParameter moveParam)
    {
        this.moveParam = moveParam;
    }
}

public class FacingDirectionMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    public class Baker : Baker<FacingDirectionMoveAuthoring>
    {
        public override void Bake(FacingDirectionMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var facingDirectionMove = new FacingDirectionMoveData(src._moveParam);

            AddComponent(entity, facingDirectionMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
