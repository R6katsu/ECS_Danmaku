using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// 目標位置まで移動する為の情報
/// </summary>
public struct MoveToTargetPointData : IComponentData
{
    [Tooltip("移動の設定値")]
    public readonly MoveParameter moveParam;

    [Tooltip("目標位置")]
    public readonly float3 targetPoint;

    /// <summary>
    /// 目標位置まで移動する為の情報
    /// </summary>
    /// <param name="moveParam">移動の設定値</param>
    /// <param name="targetPoint">目標位置</param>
    public MoveToTargetPointData(MoveParameter moveParam, float3 targetPoint)
    {
        this.moveParam = moveParam;
        this.targetPoint = targetPoint;
    }
}

/// <summary>
/// 目標位置まで移動する為の設定
/// </summary>
public class MoveToTargetPointAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField, Header("目標位置")]
    private float3 _targetPoint = float3.zero;

    public class Baker : Baker<MoveToTargetPointAuthoring>
    {
        public override void Bake(MoveToTargetPointAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var moveToTargetPointData = new MoveToTargetPointData
                (
                    src._moveParam,
                    src._targetPoint
                );

            var moveToTargetPoint = moveToTargetPointData;

            // Dataをアタッチ
            AddComponent(entity, moveToTargetPoint);
            AddComponent(entity, new MoveTag());
        }
    }
}
