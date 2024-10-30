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
/// 向いている方向に移動する為の情報
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
/// 向いている方向に移動する為の設定
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

            // 向いている方向に移動する為の情報を作成
            var facingDirectionMove = new FacingDirectionMoveData(src._moveParam);

            // Dataをアタッチ
            AddComponent(entity, facingDirectionMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
