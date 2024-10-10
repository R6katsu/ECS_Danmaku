using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// 重なりを感知する
/// </summary>
static public partial class OverlapJobs
{
    /// <summary>
    /// 球状の重なりを感知する
    /// </summary>
    [BurstCompile]
    public partial struct SphereOverlapJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;
        public float deltaTime;

        public void Execute(
            Entity entity,
            SphereOverlapData sphereOverlapData,
            LocalTransform localTfm,
            [EntityIndexInQuery] int index)
        {
            // 重なっていたら処理を実行する
            // 処理内容は不明。Actionはnullを許容しているのでDataに保持させられない

            // SphereOverlapData同士でのみ接触判定を行う
            // Box等はやり方を考えなければいけない
            // 取り合えず球状同士で当たり判定を完成させる。その後に他の人のCodeを見たりして学習する



            /*
            n_Way_DanmakuData.elapsedTime += deltaTime;

            // 発射するための時間が経過していない場合は切り上げる
            if (n_Way_DanmakuData.elapsedTime < n_Way_DanmakuData.firingInterval)
            {
                // 更新内容を再設定
                commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

                return;
            }

            n_Way_DanmakuData.elapsedTime = 0.0f;

            // 更新内容を再設定
            commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

            float fanAngle = n_Way_DanmakuData.fanAngle;
            int amountBullets = n_Way_DanmakuData.amountBullets;

            // 発射する弾の初期角度
            float startAngle = -fanAngle / 2f;
            // 1つの弾ごとの角度の増加
            float angleStep = fanAngle / (amountBullets - 1);

            for (int i = 0; i < amountBullets; i++)
            {
                var magicNumberAngle = 90.0f;   // 修正必須、向きを変える為のマジックナンバー

                // 弾の発射角度を計算
                float angle = startAngle + angleStep * i;
                quaternion rotation = (quaternion)Quaternion.Euler(angle, magicNumberAngle, magicNumberAngle);

                Entity bulletEntity = commandBuffer.Instantiate(index, n_Way_DanmakuData.bulletPrefab);

                // Transformを代入
                commandBuffer.SetComponent(index, bulletEntity, new LocalTransform
                {
                    Position = localTfm.Position,
                    Rotation = math.mul(localTfm.Rotation, rotation),
                    Scale = 0.1f
                });
            }
            */
        }
    }
}
