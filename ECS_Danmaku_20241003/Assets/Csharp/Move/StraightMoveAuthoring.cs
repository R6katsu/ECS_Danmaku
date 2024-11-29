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

    [Tooltip("進行方向")]
    public readonly AxisType axisType;

    /// <summary>
    /// 直進移動の情報
    /// </summary>
    /// <param name="moveParam">移動の設定値</param>
    /// <param name="axisType">進行方向</param>
    public StraightMoveData(MoveParameter moveParam, AxisType axisType)
    {
        this.moveParam = moveParam;
        this.axisType = axisType;
    }
}

/// <summary>
/// 直進移動の設定
/// </summary>
public class StraightMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField, Header("進行方向")]
    private AxisType _axisType = 0;

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var straightMoveData = new StraightMoveData
                (
                    src._moveParam, 
                    src._axisType
                );

            var straightMove = straightMoveData;

            // Dataをアタッチ
            AddComponent(entity, straightMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
