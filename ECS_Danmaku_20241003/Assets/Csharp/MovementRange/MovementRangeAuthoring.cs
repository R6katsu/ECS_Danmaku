using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using static MovementRangeAuthoring;
using System.Collections;
using System.Collections.Generic;
using static HealthPointDatas;
#endif

// リファクタリング済み

/// <summary>
/// 移動可能範囲と中心位置の情報
/// </summary>
[Serializable]
public struct MovementRangeInfo
{
    [Header("移動可能範囲")]
    public float3 movementRange;

    [Header("移動可能範囲の中心位置")]
    public float3 movementRangeCenter;
}

/// <summary>
/// 移動可能範囲の情報（シングルトン前提）
/// </summary>
[Serializable]
public struct MovementRangeSingletonData : IComponentData
{
    [SerializeField, Header("PLの移動可能範囲")]
    private MovementRangeInfo _playerMovementRange;

    [SerializeField, Header("弾の移動可能範囲")]
    private MovementRangeInfo _bulletMovementRange;

    [SerializeField, Header("敵の移動可能範囲")]
    private MovementRangeInfo _enemyMovementRange;

    /// <summary>
    /// PLの移動可能範囲
    /// </summary>
    public MovementRangeInfo PlayerMovementRange => _playerMovementRange;

    /// <summary>
    /// 弾の移動可能範囲
    /// </summary>
    public MovementRangeInfo BulletMovementRange => _bulletMovementRange;

    /// <summary>
    /// 敵の移動可能範囲
    /// </summary>
    public MovementRangeInfo EnemyMovementRange => _enemyMovementRange;

    /// <summary>
    /// 移動可能範囲の情報（シングルトン前提）
    /// </summary>
    /// <param name="playerMovementRange">PLの移動可能範囲</param>
    /// <param name="bulletMovementRange">弾の移動可能範囲</param>
    /// <param name="enemyMovementRange">敵の移動可能範囲</param>
    public MovementRangeSingletonData
    (
        MovementRangeInfo playerMovementRange, 
        MovementRangeInfo bulletMovementRange, 
        MovementRangeInfo enemyMovementRange
    )
    {
        _playerMovementRange = playerMovementRange;
        _bulletMovementRange = bulletMovementRange;
        _enemyMovementRange = enemyMovementRange;
    }
}

/// <summary>
/// 移動可能範囲の設定
/// </summary>
public class MovementRangeAuthoring : SingletonMonoBehaviour<MovementRangeAuthoring>
{
    [SerializeField, Header("移動可能範囲の情報")]
    private MovementRangeSingletonData _playerMovementRangeSingletonData = new();

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
