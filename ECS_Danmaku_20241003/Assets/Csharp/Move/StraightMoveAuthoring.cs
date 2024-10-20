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
/// 直進移動の情報
/// </summary>
public struct StraightMoveData : IComponentData
{
    [Tooltip("移動の設定値")]
    public readonly MoveParameter moveParam;

    /// <summary>
    /// 直進移動の情報
    /// </summary>
    /// <param name="moveParam">移動の設定値</param>
    public StraightMoveData(MoveParameter moveParam)
    {
        this.moveParam = moveParam;
    }
}

/// <summary>
/// 直進移動の設定
/// </summary>
public class StraightMoveAuthoring : MonoBehaviour
{
    // 範囲から出ると削除されるようにする
    // AABBとかfloat3とかを持ったTransform？

    [SerializeField]
    private MoveParameter _moveParam = new();

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var straightMove = new StraightMoveData(src._moveParam);

            // Dataをアタッチ
            AddComponent(entity, straightMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
