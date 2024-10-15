using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// ’e–‹‚ÌJobSystem
/// </summary>
static public partial class DanmakuJobs
{
    /// <summary>
    /// N_Way’e‚ğ¶¬
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
            // Œo‰ßŠÔ‚ğ‰ÁZ‘ã“ü
            n_Way_DanmakuData.elapsedTime += deltaTime;

            // ”­Ë‚·‚é‚½‚ß‚ÌŠÔ‚ªŒo‰ß‚µ‚Ä‚¢‚È‚¢ê‡‚ÍØ‚èã‚°‚é
            if (n_Way_DanmakuData.elapsedTime < n_Way_DanmakuData.firingInterval) 
            {
                // XV‚ğ”½‰f
                commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

                return; 
            }

            n_Way_DanmakuData.elapsedTime = 0.0f;

            // XV‚ğ”½‰f
            commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

            float fanAngle = n_Way_DanmakuData.fanAngle;
            int amountBullets = n_Way_DanmakuData.amountBullets;

            // ”­Ë‚·‚é’e‚Ì‰ŠúŠp“x
            float startAngle = -fanAngle / 2f;

            // 1‚Â‚Ì’e‚²‚Æ‚ÌŠp“x‚Ì‘‰Á
            float angleStep = fanAngle / (amountBullets);

            // n-Way’e‚ğ¶¬‚·‚é
            for (int i = 0; i < amountBullets; i++)
            {
                // ’e‚Ì”­ËŠp“x‚ğŒvZ
                float angle = startAngle + angleStep * i;
                quaternion rotation = (quaternion)Quaternion.Euler(0.0f, angle, 0.0f);

                // ’e‚ğ¶¬
                Entity bulletEntity = commandBuffer.Instantiate(index, n_Way_DanmakuData.bulletPrefab);

                // ³‹K‰»
                quaternion normalizedRotation = math.normalize(localTfm.Rotation);

                // Transform‚ğ‘ã“ü
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
