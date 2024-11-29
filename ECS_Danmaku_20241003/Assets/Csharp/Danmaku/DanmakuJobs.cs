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
//[BurstCompile]
static public partial class DanmakuJobs
{
    /// <summary>
    /// N_Way弾を生成
    /// </summary>
    //[BurstCompile]
    public partial struct N_WayJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;
        public float deltaTime;

        public void Execute(
            Entity entity,
            N_Way_DanmakuData n_Way_DanmakuData,
            LocalTransform localTfm,
            LocalToWorld localToWorld,
            [EntityIndexInQuery] int index)
        {
            if (n_Way_DanmakuData.IsDataDeletion)
            {
                commandBuffer.RemoveComponent<N_Way_DanmakuData>(index, entity);
                return;
            }

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
                Entity bulletEntity = commandBuffer.Instantiate(index, n_Way_DanmakuData.bulletEntity);

                // 正規化
                quaternion normalizedRotation = math.normalize(localToWorld.Rotation);

                // Transformを代入
                commandBuffer.SetComponent(index, bulletEntity, new LocalTransform
                {
                    Position = localToWorld.Position,
                    Rotation = math.mul(normalizedRotation, rotation),
                    Scale = n_Way_DanmakuData.bulletLocalScale
                });
            }
        }
    }

    /// <summary>
    /// タップ撃ち弾幕を生成
    /// </summary>
    //[BurstCompile]
    public partial struct TapShootingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;
        public double elapsedTime;

        public void Execute(
            Entity entity,
            TapShooting_DanmakuData tapShooting_DanmakuData,
            LocalTransform localTfm,
            [EntityIndexInQuery] int index)
        {
            if (tapShooting_DanmakuData.IsDataDeletion)
            {
                commandBuffer.RemoveComponent<TapShooting_DanmakuData>(index, entity);
                return;
            }

            // 現在の時刻が次回のワンセット開始時刻未満だった
            if (elapsedTime < tapShooting_DanmakuData.singleSetNextTime) { return; }

            // 現在の時刻が次回の射撃時刻未満だった
            if (elapsedTime < tapShooting_DanmakuData.firingNextTime) { return; }

            // 弾を生成
            Entity bulletEntity = commandBuffer.Instantiate(index, tapShooting_DanmakuData.bulletEntity);

            // Transformを代入
            commandBuffer.SetComponent(index, bulletEntity, new LocalTransform
            {
                Position = localTfm.Position,
                Rotation = localTfm.Rotation,
                Scale = 1.0f
            });

            // 現在の射撃回数をインクリメント
            tapShooting_DanmakuData.currentShotCount++;

            // 現在の射撃回数がワンセットの射撃回数以上だった
            if (tapShooting_DanmakuData.currentShotCount >= tapShooting_DanmakuData.shootNSingleSet)
            {
                //現在の射撃回数をリセット
                tapShooting_DanmakuData.currentShotCount = 0;

                // 次回のワンセット開始時刻を更新する
                tapShooting_DanmakuData.singleSetNextTime = elapsedTime + tapShooting_DanmakuData.singleSetRestTimeAfter;

                // 次回の射撃時刻を更新する
                // 次回のワンセット開始時刻 + （射撃間隔 * 現在の射撃回数） = 次回の射撃時刻
                tapShooting_DanmakuData.firingNextTime = tapShooting_DanmakuData.singleSetNextTime + tapShooting_DanmakuData.firingInterval * tapShooting_DanmakuData.currentShotCount;
            }

            // 更新を反映
            commandBuffer.SetComponent(index, entity, tapShooting_DanmakuData);
        }
    }
}
