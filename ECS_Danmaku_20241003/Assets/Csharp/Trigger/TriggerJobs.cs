using Unity.Entities;
using Unity.Physics;
using static BulletHelper;
using static EntityCampsHelper;
using static HealthPointDatas;
using Unity.Transforms;

#if UNITY_EDITOR
using static EnemyHelper;
using static PlayerHelper;
using Unity.Burst;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using static HealthHelper;
using static PlayerAuthoring;
using static TriggerHelper;
using System;
#endif

/// <summary>
/// �ڐG�����ۂ̏���
/// </summary>
//[BurstCompile]
static public partial class TriggerJobs
{
    /// <summary>
    /// �ڐG���Ƀ_���[�W���󂯂�
    /// </summary>
    //[BurstCompile]
    public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
    {
        // �C���X�^���X�̎擾�ɕK�v�ȕϐ�
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;
        public ComponentLookup<RemainingPierceCountData> remainingPierceCountLookup;
        public ComponentLookup<LocalTransform> localTransformLookup;
        public ComponentLookup<VFXCreationData> vfxCreationLookup;
        public ComponentLookup<AudioPlayData> audioPlayLookup;

        // ���݂̎���
        public double currentTime;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
            var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

            // entityB��LocalTransform��L���Ă��邩
            if (!localTransformLookup.HasComponent(entityB)) { return; }

            // isTrigger���L���ł���ȏ�ALocalTransform�͐�΂ɂ���
            var localTransform = localTransformLookup[entityB];
            var position = localTransform.Position;

            // entityA��BulletIDealDamageData��L���Ă��Ȃ��B
            // ���邢�́AentityB��PlayerHealthPointData��L���Ă��Ȃ���ΐ؂�グ��
            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            // entityB����PlayerHealthPointData���擾
            var healthPoint = healthPointLookup[entityB];

            // �O���e�������Ԃ��疳�G���Ԃ̒������o�߂��Ă��Ȃ�
            if (currentTime - healthPoint.lastHitTime <= healthPoint.isInvincibleTime) { return; }

            // entityA����BulletIDealDamageData���擾
            var dealDamage = dealDamageLookup[entityA];

            // �_���[�W���̐w�c�̎�ނ�Player��������؂�グ��
            if (dealDamage.campsType == EntityCampsType.Player) { return; }

            // �_���[�W��^���A�ύX���ꂽ�C���X�^���X�𔽉f����
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // �_���[�W����RemainingPierceCountData��L���Ă���
            if (remainingPierceCountLookup.HasComponent(entityA))
            {
                // �c��ђʉ񐔂��f�N�������g
                var remainingPierceCount = remainingPierceCountLookup[entityA];
                remainingPierceCount.remainingPierceCount--;
                remainingPierceCountLookup[entityA] = remainingPierceCount;

                // �_���[�W����DestroyableData��L���Ă���
                if (destroyableLookup.HasComponent(entityA))
                {
                    var destroyable = destroyableLookup[entityA];

                    // �c��ђʉ񐔂�0�ȉ���
                    destroyable.isKilled = (remainingPierceCount.remainingPierceCount <= 0) ? true : false;

                    // �ύX�𔽉f
                    destroyableLookup[entityA] = destroyable;
                }
            }

            // �U�����󂯂��Ώۂ�DestroyableData��L���Ă���
            if (destroyableLookup.HasComponent(entityB))
            {
                var destroyable = destroyableLookup[entityB];

                var isKilled = healthPoint.isKilled;

                // HP�̍폜�t���O��������
                destroyable.isKilled = isKilled;

                // �ύX�𔽉f
                destroyableLookup[entityB] = destroyable;

                // �폜�t���O��������
                if (isKilled)
                {
                    // �Q�[���I�[�o�[�������J�n����
                    GameManager.Instance.MyGameState = GameState.GameOver;

                    // EntityB��VFXCreationData��L���Ă���
                    if (vfxCreationLookup.HasComponent(entityB))
                    {
                        var vfxCreation = vfxCreationLookup[entityB];
                        vfxCreation.Position = position;
                        vfxCreationLookup[entityB] = vfxCreation;
                    }

                    // EntityB��AudioPlayData��L���Ă���
                    if (audioPlayLookup.HasComponent(entityB))
                    {
                        var audioPlay = audioPlayLookup[entityB];
                        audioPlay.AudioNumber = healthPoint.killedSENumber;
                        audioPlayLookup[entityB] = audioPlay;
                    }
                }
            }
        }
    }

    /// <summary>
    /// �ڐG���Ƀ_���[�W���󂯂�
    /// </summary>
    //[BurstCompile]
    public partial struct EnemyDamageTriggerJob : ITriggerEventsJob
    {
        // �C���X�^���X�̎擾�ɕK�v�ȕϐ�
        public ComponentLookup<EnemyHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;
        public ComponentLookup<RemainingPierceCountData> remainingPierceCountLookup;
        public ComponentLookup<LocalTransform> localTransformLookup;
        public ComponentLookup<VFXCreationData> vfxCreationLookup;
        public ComponentLookup<AudioPlayData> audioPlayLookup;
        public ComponentLookup<BossEnemyCampsTag> bossEnemyCampsLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
            var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

            // entityB��LocalTransform��L���Ă��邩
            if (!localTransformLookup.HasComponent(entityB)) { return; }

            var localTransform = localTransformLookup[entityB];
            var position = localTransform.Position;

            // entityA��BulletIDealDamageData��L���Ă��Ȃ��B
            // ���邢�́AentityB��EnemyHealthPointData��L���Ă��Ȃ���ΐ؂�グ��
            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            // entityB����EnemyHealthPointData���擾
            var healthPoint = healthPointLookup[entityB];

            // entityA����BulletIDealDamageData���擾
            var dealDamage = dealDamageLookup[entityA];

            // �_���[�W���̐w�c�̎�ނ�Enemy��������؂�グ��
            if (dealDamage.campsType == EntityCampsType.Enemy) { return; }

            // �_���[�W��^���A�ύX���ꂽ�C���X�^���X�𔽉f����
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // �_���[�W����RemainingPierceCountData��L���Ă���
            if (remainingPierceCountLookup.HasComponent(entityA))
            {
                // �c��ђʉ񐔂��f�N�������g
                var remainingPierceCount = remainingPierceCountLookup[entityA];
                remainingPierceCount.remainingPierceCount--;
                remainingPierceCountLookup[entityA] = remainingPierceCount;

                // �_���[�W����DestroyableData��L���Ă���
                if (destroyableLookup.HasComponent(entityA))
                {
                    var destroyable = destroyableLookup[entityA];

                    // �c��ђʉ񐔂�0�ȉ���
                    destroyable.isKilled = (remainingPierceCount.remainingPierceCount <= 0) ? true : false;

                    // �ύX�𔽉f
                    destroyableLookup[entityA] = destroyable;
                }
            }

            // �U�����󂯂��Ώۂ�DestroyableData��L���Ă���
            if (destroyableLookup.HasComponent(entityB))
            {
                var destroyable = destroyableLookup[entityB];

                var isKilled = healthPoint.isKilled;

                // HP�̍폜�t���O��������
                destroyable.isKilled = isKilled;

                // �ύX�𔽉f
                destroyableLookup[entityB] = destroyable;

                // EntityB��AudioPlayData��L���Ă���
                if (audioPlayLookup.HasComponent(entityB))
                {
                    var audioPlay = audioPlayLookup[entityB];

                    // �폜�t���O�������Ă����玀�S���̌��ʉ��A�����Ă��Ȃ���Δ�e���̌��ʉ�����
                    audioPlay.AudioNumber = (isKilled) ? healthPoint.killedSENumber : healthPoint.hitSENumber;

                    audioPlayLookup[entityB] = audioPlay;
                }

                // �|���ꂽ
                if (isKilled)
                {
                    // EntityB��VFXCreationData��L���Ă���
                    if (vfxCreationLookup.HasComponent(entityB))
                    {
                        var vfxCreation = vfxCreationLookup[entityB];

                        vfxCreation.Position = position;
                        vfxCreationLookup[entityB] = vfxCreation;
                    }

                    // �{�X�G������
                    if (bossEnemyCampsLookup.HasComponent(entityB))
                    {
                        // �{�X���|���ꂽ���߃Q�[���N���A
                        GameManager.Instance.MyGameState = GameState.GameClear;
                    }
                }
            }
        }
    }
}
