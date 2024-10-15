using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 弾幕のJobSystem
/// </summary>
static public partial class DanmakuJobs
{
    /// <summary>
    /// N_Way弾を生成
    /// </summary>
    [BurstCompile]
    public partial struct N_WayJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;
        public float deltaTime;

        public void Execute(
            Entity entity,
            N_Way_DanmakuData n_Way_DanmakuData,
            LocalTransform localTfm,
            [EntityIndexInQuery] int index)
        {
            // 経過時間を加算代入
            n_Way_DanmakuData.elapsedTime += deltaTime;

            // 発射するための時間が経過していない場合は切り上げる
            if (n_Way_DanmakuData.elapsedTime < n_Way_DanmakuData.firingInterval) 
            {
                // 更新を反映
                commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

                return; 
            }

            n_Way_DanmakuData.elapsedTime = 0.0f;

            // 更新を反映
            commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

            float fanAngle = n_Way_DanmakuData.fanAngle;
            int amountBullets = n_Way_DanmakuData.amountBullets;

            // 発射する弾の初期角度
            float startAngle = -fanAngle / 2f;

            // 1つの弾ごとの角度の増加
            float angleStep = fanAngle / (amountBullets);

            // n-Way弾を生成する
            for (int i = 0; i < amountBullets; i++)
            {
                // 弾の発射角度を計算
                float angle = startAngle + angleStep * i;
                quaternion rotation = (quaternion)Quaternion.Euler(0.0f, angle, 0.0f);

                // 弾を生成
                Entity bulletEntity = commandBuffer.Instantiate(index, n_Way_DanmakuData.bulletPrefab);

                // 正規化
                quaternion normalizedRotation = math.normalize(localTfm.Rotation);

                // Transformを代入
                commandBuffer.SetComponent(index, bulletEntity, new LocalTransform
                {
                    Position = localTfm.Position,
                    Rotation = math.mul(normalizedRotation, rotation),
                    Scale = n_Way_DanmakuData.bulletLocalScale
                });
            }
        }
    }
}
