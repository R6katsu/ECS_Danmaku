using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// �d�Ȃ�����m����
/// </summary>
static public partial class OverlapJobs
{
    /// <summary>
    /// ����̏d�Ȃ�����m����
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
            // �d�Ȃ��Ă����珈�������s����
            // �������e�͕s���BAction��null�����e���Ă���̂�Data�ɕێ��������Ȃ�

            // SphereOverlapData���m�ł̂ݐڐG������s��
            // Box���͂������l���Ȃ���΂����Ȃ�
            // ��荇�������󓯎m�œ����蔻�������������B���̌�ɑ��̐l��Code�������肵�Ċw�K����



            /*
            n_Way_DanmakuData.elapsedTime += deltaTime;

            // ���˂��邽�߂̎��Ԃ��o�߂��Ă��Ȃ��ꍇ�͐؂�グ��
            if (n_Way_DanmakuData.elapsedTime < n_Way_DanmakuData.firingInterval)
            {
                // �X�V���e���Đݒ�
                commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

                return;
            }

            n_Way_DanmakuData.elapsedTime = 0.0f;

            // �X�V���e���Đݒ�
            commandBuffer.SetComponent(index, entity, n_Way_DanmakuData);

            float fanAngle = n_Way_DanmakuData.fanAngle;
            int amountBullets = n_Way_DanmakuData.amountBullets;

            // ���˂���e�̏����p�x
            float startAngle = -fanAngle / 2f;
            // 1�̒e���Ƃ̊p�x�̑���
            float angleStep = fanAngle / (amountBullets - 1);

            for (int i = 0; i < amountBullets; i++)
            {
                var magicNumberAngle = 90.0f;   // �C���K�{�A������ς���ׂ̃}�W�b�N�i���o�[

                // �e�̔��ˊp�x���v�Z
                float angle = startAngle + angleStep * i;
                quaternion rotation = (quaternion)Quaternion.Euler(angle, magicNumberAngle, magicNumberAngle);

                Entity bulletEntity = commandBuffer.Instantiate(index, n_Way_DanmakuData.bulletPrefab);

                // Transform����
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
