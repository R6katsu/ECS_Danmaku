using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MovementRangeAuthoring;

#if UNITY_EDITOR
using static HealthPointDatas;
using static PlayerHelper;
#endif

/// <summary>
/// 移動可能範囲の情報（シングルトン前提）
/// </summary>
[Serializable]
public struct MovementRangeSingletonData : IComponentData
{
    [Header("PLの移動可能範囲")]
    public MovementRangeInfo playerMovementRange;

    [Header("弾の移動可能範囲")]
    public MovementRangeInfo bulletMovementRange;

    [Header("敵の移動可能範囲")]
    public MovementRangeInfo enemyMovementRange;
}

/// <summary>
/// 移動可能範囲の設定
/// </summary>
public class MovementRangeAuthoring : SingletonMonoBehaviour<MovementRangeAuthoring>
{
    /// <summary>
    /// 移動可能範囲と中心位置の情報
    /// </summary>
    [Serializable]
    public struct MovementRangeInfo
    {
        [SerializeField, Header("移動可能範囲")]
        public float3 movementRange;

        [SerializeField, Header("移動可能範囲の中心位置")]
        public float3 movementRangeCenter;
    }

    public MovementRangeSingletonData _playerMovementRangeSingletonData = new();

    public class Baker : Baker<MovementRangeAuthoring>
    {
        public override void Bake(MovementRangeAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, src._playerMovementRangeSingletonData);

            // ビルド時に固定
            AddComponent<StaticOptimizeEntity>(entity);
        }
    }
}
