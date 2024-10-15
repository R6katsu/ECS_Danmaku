using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// �e����JobSystem
/// </summary>
static public partial class DanmakuJobs
{
    /// <summary>
    /// N_Way�e�𐶐�
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
            // �o�ߎ��Ԃ����Z���
            n_Way_DanmakuData.elapsedTime += deltaTime;

            // ���˂��邽�߂̎��Ԃ��o�߂��Ă��Ȃ��ꍇ�͐؂�グ��
            if (n_Way_DanmakuData.elapsedTime < n_Way_DanmakuData.firingInterval) 
            {
                // �X�V�𔽉f
                commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

                return; 
            }

            n_Way_DanmakuData.elapsedTime = 0.0f;

            // �X�V�𔽉f
            commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

            float fanAngle = n_Way_DanmakuData.fanAngle;
            int amountBullets = n_Way_DanmakuData.amountBullets;

            // ���˂���e�̏����p�x
            float startAngle = -fanAngle / 2f;

            // 1�̒e���Ƃ̊p�x�̑���
            float angleStep = fanAngle / (amountBullets);

            // n-Way�e�𐶐�����
            for (int i = 0; i < amountBullets; i++)
            {
                // �e�̔��ˊp�x���v�Z
                float angle = startAngle + angleStep * i;
                quaternion rotation = (quaternion)Quaternion.Euler(0.0f, angle, 0.0f);

                // �e�𐶐�
                Entity bulletEntity = commandBuffer.Instantiate(index, n_Way_DanmakuData.bulletPrefab);

                // ���K��
                quaternion normalizedRotation = math.normalize(localTfm.Rotation);

                // Transform����
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
