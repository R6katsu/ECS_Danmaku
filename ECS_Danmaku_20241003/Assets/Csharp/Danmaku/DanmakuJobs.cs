using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �e����JobSystem
/// </summary>
[BurstCompile]
static public partial class DanmakuJobs
{
    /// <summary>
    /// N_Way�e�𐶐�
    /// </summary>
    [BurstCompile]
    public partial struct N_WayJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;

        [Tooltip("�t���[���b")]
        public float deltaTime;

        public void Execute(
            Entity entity,
            NWay_DanmakuData n_Way_DanmakuData,
            LocalTransform localTfm,
            LocalToWorld localToWorld,
            [EntityIndexInQuery] int index)
        {
            // �t���[���b�����Z���
            n_Way_DanmakuData.elapsedTime += deltaTime;

            // ���˂��邽�߂̎��Ԃ��o�߂��Ă��Ȃ��ꍇ�͐؂�グ��
            if (n_Way_DanmakuData.elapsedTime < n_Way_DanmakuData.firingInterval) 
            {
                // ���Z��������t���[���b�𔽉f
                commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);
                return; 
            }

            // �o�ߎ��Ԃ����������Ĉȍ~�͔��ˏ���
            n_Way_DanmakuData.elapsedTime = 0.0f;

            // �o�ߎ��Ԃ̏������𔽉f
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
                Entity bulletEntity = commandBuffer.Instantiate(index, n_Way_DanmakuData.bulletEntity);

                // ���K��
                quaternion normalizedRotation = math.normalize(localToWorld.Rotation);

                // Transform��ݒ�
                commandBuffer.SetComponent(index, bulletEntity, new LocalTransform
                {
                    Position = localToWorld.Position,
                    Rotation = math.mul(normalizedRotation, rotation),
                    Scale = localTfm.Scale
                });
            }
        }
    }

    /// <summary>
    /// �^�b�v�����e���𐶐�
    /// </summary>
    [BurstCompile]
    public partial struct TapShootingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;

        [Tooltip("�o�ߎ���")]
        public double elapsedTime;

        public void Execute(
            Entity entity,
            TapShooting_DanmakuData tapShooting_DanmakuData,
            LocalTransform localTfm,
            [EntityIndexInQuery] int index)
        {
            // ���݂̎���������̃����Z�b�g�J�n��������������
            if (elapsedTime < tapShooting_DanmakuData.singleSetNextTime) { return; }

            // ���݂̎���������̎ˌ���������������
            if (elapsedTime < tapShooting_DanmakuData.firingNextTime) { return; }

            // �e�𐶐�
            Entity bulletEntity = commandBuffer.Instantiate(index, tapShooting_DanmakuData.bulletEntity);

            // Transform��ݒ�
            commandBuffer.SetComponent(index, bulletEntity, new LocalTransform
            {
                Position = localTfm.Position,
                Rotation = localTfm.Rotation,
                Scale = localTfm.Scale
            });

            // ���݂̎ˌ��񐔂��C���N�������g
            tapShooting_DanmakuData.currentShotCount++;

            // ���݂̎ˌ��񐔂������Z�b�g�̎ˌ��񐔈ȏゾ����
            if (tapShooting_DanmakuData.currentShotCount >= tapShooting_DanmakuData.shootNSingleSet)
            {
                //���݂̎ˌ��񐔂�������
                tapShooting_DanmakuData.currentShotCount = 0;

                // ����̃����Z�b�g�J�n�������X�V����
                tapShooting_DanmakuData.singleSetNextTime = elapsedTime + tapShooting_DanmakuData.singleSetRestTimeAfter;

                // ����̎ˌ��������X�V����
                // ����̃����Z�b�g�J�n���� + �i�ˌ��Ԋu * ���݂̎ˌ��񐔁j = ����̎ˌ�����
                tapShooting_DanmakuData.firingNextTime = tapShooting_DanmakuData.singleSetNextTime + tapShooting_DanmakuData.firingInterval * tapShooting_DanmakuData.currentShotCount;
            }

            // �X�V�𔽉f
            commandBuffer.SetComponent(index, entity, tapShooting_DanmakuData);
        }
    }
}
