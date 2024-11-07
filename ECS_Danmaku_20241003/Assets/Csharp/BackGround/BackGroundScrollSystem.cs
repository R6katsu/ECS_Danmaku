using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
#endif

/// <summary>
/// 背景スクロールの処理
/// </summary>
[BurstCompile]
public partial struct BackGroundScrollSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        foreach (var (backGroundScroll, localTfm) in
                 SystemAPI.Query
                 <RefRO<BackGroundScrollData>,
                 RefRW<LocalTransform>>())
        {
            var initialPosition = backGroundScroll.ValueRO.initialPosition;
            var startPosition = backGroundScroll.ValueRO.startPosition;
            var endLength = backGroundScroll.ValueRO.endLength;
            var axisType = backGroundScroll.ValueRO.axisType;

            // 今回のフレームでの移動量
            var frameMovement = Time.deltaTime * backGroundScroll.ValueRO.moveDirection;

            // 目標に到達したか
            bool hasReachedTarget = false;

            // 移動方向が反転しているか
            bool isMovingBackwards = math.length(startPosition) > 0.0f;

            // 反転していたら符号を反転させる
            endLength *= (isMovingBackwards) ? -1.0f : 1.0f;

            // AxisTypeに対応する各成分を取得
            var initialPositionValue = AxisTypeHelper.GetAxisValue(axisType, initialPosition);
            var localPositionValue = AxisTypeHelper.GetAxisValue(axisType, localTfm.ValueRO.Position);
            var frameMovementValue = AxisTypeHelper.GetAxisValue(axisType, frameMovement);

            // 移動方向によって終了距離と現在の移動距離を変える
            float end = (isMovingBackwards) ? initialPositionValue + endLength : localPositionValue + frameMovementValue;
            float current = (isMovingBackwards) ? localPositionValue + frameMovementValue : initialPositionValue + endLength;

            // 目標に到達したか
            hasReachedTarget = end >= current;

            // 目標に到達した
            if (hasReachedTarget)
            {
                // 移動開始位置に移動
                localTfm.ValueRW.Position = startPosition;
            }
            else
            {
                // 移動を反映
                localTfm.ValueRW.Position += frameMovement;
            }
        }
    }
}
