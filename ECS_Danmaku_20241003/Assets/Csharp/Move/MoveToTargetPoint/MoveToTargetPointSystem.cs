using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

#if UNITY_EDITOR
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// 目標位置まで移動する処理
/// </summary>
[BurstCompile]
public partial struct MoveToTargetPointSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;
        var entityManager = state.EntityManager;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // 目標位置まで移動する処理
        foreach (var (moveToTargetPoint, localTfm, entity) in
            SystemAPI.Query<RefRO<MoveToTargetPointData>,
                                 RefRW<LocalTransform>>()
                                 .WithEntityAccess())
        {
            // 現在位置を取得
            var currentPosition = localTfm.ValueRW.Position;

            // 目標位置を取得
            var targetPosition = moveToTargetPoint.ValueRO.TargetPoint;

            // 移動速度を取得
            var moveSpeed = moveToTargetPoint.ValueRO.MoveParameter.Speed;

            // 移動方向を計算
            var direction = math.normalize(targetPosition - currentPosition);

            // フレームごとに移動距離を計算
            var deltaMove = moveSpeed * delta;

            // 新しい位置を計算
            var newPosition = currentPosition + direction * deltaMove;

            // 目標地点を超えた
            if (math.distance(newPosition, targetPosition) < deltaMove)
            {
                // 必要なくなったMoveToTargetPointDataを削除
                ecb.RemoveComponent<MoveToTargetPointData>(entity);
                continue;
            }

            // 現在位置を更新
            localTfm.ValueRW.Position = newPosition;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
