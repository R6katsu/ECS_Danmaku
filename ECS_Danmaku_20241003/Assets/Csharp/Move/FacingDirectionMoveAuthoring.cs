using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
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
            var facingDirectionMove = new FacingDirectionMoveData(src._moveParam);

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), facingDirectionMove);
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new MoveTag());
        }
    }
}
