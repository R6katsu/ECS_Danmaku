using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;
using System;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// 目標位置まで移動する為の情報
/// </summary>
[Serializable]
public struct MoveToTargetPointData : IComponentData
{
    [SerializeField, Header("移動の設定値")]
    private MoveParameter _moveParameter;

    [SerializeField, Header("目標位置")]
    private float3 _targetPoint;

    /// <summary>
    /// 移動の設定値
    /// </summary>
    public MoveParameter MoveParameter => _moveParameter;

    /// <summary>
    /// 目標位置
    /// </summary>
    public float3 TargetPoint => _targetPoint;

    /// <summary>
    /// 目標位置まで移動する為の情報
    /// </summary>
    /// <param name="moveParameter">移動の設定値</param>
    /// <param name="targetPoint">目標位置</param>
    public MoveToTargetPointData(MoveParameter moveParameter, float3 targetPoint)
    {
        _moveParameter = moveParameter;
        _targetPoint = targetPoint;
    }
}

/// <summary>
/// 目標位置まで移動する為の設定
/// </summary>
public class MoveToTargetPointAuthoring : MonoBehaviour
{
    [SerializeField, Header("目標位置まで移動する為の情報")]
    private MoveToTargetPointData _moveToTargetPointData = new();

    public class Baker : Baker<MoveToTargetPointAuthoring>
    {
        public override void Bake(MoveToTargetPointAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Dataをアタッチ
            AddComponent(entity, src._moveToTargetPointData);
            AddComponent(entity, new MoveTag());
        }
    }
}
