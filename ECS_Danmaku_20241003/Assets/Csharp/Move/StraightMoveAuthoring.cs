using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

public struct StraightMoveData : IComponentData
{
    public readonly MoveParameter moveParam;

    public StraightMoveData(MoveParameter moveParam)
    {
        this.moveParam = moveParam;
    }
}

public class StraightMoveAuthoring : MonoBehaviour
{
    // ”ÍˆÍ‚©‚ço‚é‚Æíœ‚³‚ê‚é‚æ‚¤‚É‚·‚é

    [SerializeField]
    private MoveParameter _moveParam = new();

    public MoveParameter MoveParam => _moveParam;

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var straightMove = new StraightMoveData(src.MoveParam);

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), straightMove);
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new MoveTag());
        }
    }
}
