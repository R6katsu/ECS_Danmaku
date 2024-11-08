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

    /*
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
        this.playerMovementRange = playerMovementRange;
        this.bulletMovementRange = bulletMovementRange;
        this.enemyMovementRange = enemyMovementRange;
    }
    */
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

    /*
    [SerializeField, Header("PLの移動可能範囲")]
    private MovementRangeInfo _playerMovementRange = new();

    [SerializeField, Header("弾の移動可能範囲")]
    private MovementRangeInfo _bulletMovementRange = new();

    [SerializeField, Header("敵の移動可能範囲")]
    private MovementRangeInfo _enemyMovementRange = new();
    */

    public class Baker : Baker<MovementRangeAuthoring>
    {
        public override void Bake(MovementRangeAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            /*
            // MovementRangeSingletonDataのインスタンスを作成
            var movementRangeSingleton = new MovementRangeSingletonData
                (
                    src._playerMovementRange,
                    src._bulletMovementRange,
                    src._enemyMovementRange
                );
            */

            //AddComponent(entity, movementRangeSingleton);
            AddComponent(entity, src._playerMovementRangeSingletonData);

            // ビルド時に固定
            AddComponent<StaticOptimizeEntity>(entity);
        }
    }
}
