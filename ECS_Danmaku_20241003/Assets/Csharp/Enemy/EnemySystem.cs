using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static EnemyHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletHelper;
#endif

// リファクタリング済み

/// <summary>
/// EnemyTagを有するEntityの処理
/// </summary>
[BurstCompile]
public partial struct EnemySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // MovementRangeSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // シングルトンデータの取得
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // 敵の移動可能範囲を取得
        var enemyMovementRange = movementRangeSingleton.EnemyMovementRange;

        // 中心位置
        var movementRangeCenter = enemyMovementRange.movementRangeCenter;

        // 半分の大きさを求める
        var halfMovementRange = enemyMovementRange.movementRange.Halve();

        // 中心位置を考慮した移動可能範囲を求める
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // 移動可能範囲外だったら削除フラグを立てる
        foreach (var (enemy, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<EnemyTag>,
                 RefRW<DestroyableData>,
                 RefRW<LocalTransform>>())
        {
            var position = localTfm.ValueRO.Position;

            // 移動可能範囲外だった
            if (position.x < minMovementRange.x || position.x > maxMovementRange.x ||
                position.y < minMovementRange.y || position.y > maxMovementRange.y ||
                position.z < minMovementRange.z || position.z > maxMovementRange.z)
            {
                // 削除フラグを立てる
                destroyable.ValueRW.isKilled = true;
            }
        }
    }
}
