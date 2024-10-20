using Unity.Entities;
using Unity.Physics;
using static BulletHelper;
using static EntityCampsHelper;
using static EnemyHelper;
using static PlayerHelper;
using Unity.Burst;


#if UNITY_EDITOR
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

        // ���݂̎���
        public double currentTime;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
            var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

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

                // HP�̍폜�t���O��������
                destroyable.isKilled = healthPoint.isKilled;

                // �ύX�𔽉f
                destroyableLookup[entityB] = destroyable;
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

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
            var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

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

                // HP�̍폜�t���O��������
                destroyable.isKilled = healthPoint.isKilled;

                // �ύX�𔽉f
                destroyableLookup[entityB] = destroyable;
            }
        }
    }
}
