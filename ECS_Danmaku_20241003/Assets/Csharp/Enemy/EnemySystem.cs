using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static BulletHelper;
using static EnemyHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// 敵の処理
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
        var enemyMovementRange = movementRangeSingleton.enemyMovementRange;
        var movementRange = enemyMovementRange.movementRange + enemyMovementRange.movementRangeCenter;

        // 半分の大きさを求める
        var halfMovementRange = movementRange / 2;

        // 移動可能範囲外だったら削除フラグを立てる
        foreach (var (enemy, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<EnemyTag>,
                 RefRW<DestroyableData>,
                 RefRW<LocalTransform>>())
        {
            var position = localTfm.ValueRO.Position;

            // 移動可能範囲外だった
            if (position.x < -halfMovementRange.x || position.x > halfMovementRange.x ||
                position.y < -halfMovementRange.y || position.y > halfMovementRange.y ||
                position.z < -halfMovementRange.z || position.z > halfMovementRange.z)
            {
                // 削除フラグを立てる
                destroyable.ValueRW.isKilled = true;
            }
        }
    }
}
