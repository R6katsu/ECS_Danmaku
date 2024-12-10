using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static BulletHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
#endif

// リファクタリング済み

/// <summary>
/// 弾の処理
/// </summary>
[BurstCompile]
public partial struct BulletSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // MovementRangeSingletonDataが存在しなかった
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // シングルトンデータの取得
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // 弾の移動可能範囲を取得
        var bulletMovementRange = movementRangeSingleton.BulletMovementRange;

        // 中心位置
        var movementRangeCenter = bulletMovementRange.movementRangeCenter;

        // 半分の大きさを求める
        var halfMovementRange = bulletMovementRange.movementRange / 2;

        // 中心位置を考慮した移動可能範囲を求める
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // 移動可能範囲外だったら削除フラグを立てる
        foreach (var (bullet, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<BulletTag>,
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
