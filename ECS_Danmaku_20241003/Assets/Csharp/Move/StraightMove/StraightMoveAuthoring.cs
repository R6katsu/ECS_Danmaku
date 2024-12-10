using Unity.Entities;
using System;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static BulletHelper;
using static UnityEngine.EventSystems.EventTrigger;
#endif

// リファクタリング済み

/// <summary>
/// 直進移動の情報
/// </summary>
[Serializable]
public struct StraightMoveData : IComponentData
{
    [SerializeField, Header("移動の設定値")]
    private MoveParameter _moveParam;

    [SerializeField, Header("進行方向")]
    private AxisType _axisType;

    /// <summary>
    /// 移動の設定値
    /// </summary>
    public MoveParameter MoveParameter => _moveParam;

    /// <summary>
    /// 進行方向
    /// </summary>
    public AxisType AxisType => _axisType;

    /// <summary>
    /// 直進移動の情報
    /// </summary>
    /// <param name="moveParam">移動の設定値</param>
    /// <param name="axisType">進行方向</param>
    public StraightMoveData(MoveParameter moveParam, AxisType axisType)
    {
        _moveParam = moveParam;
        _axisType = axisType;
    }
}

/// <summary>
/// 直進移動の設定
/// </summary>
public class StraightMoveAuthoring : MonoBehaviour
{
    [SerializeField, Header("直進移動の情報")]
    private StraightMoveData _straightMoveData = new();

    public class Baker : Baker<StraightMoveAuthoring>
    {
        public override void Bake(StraightMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Dataをアタッチ
            AddComponent(entity, src._straightMoveData);
            AddComponent(entity, new MoveTag());
        }
    }
}
