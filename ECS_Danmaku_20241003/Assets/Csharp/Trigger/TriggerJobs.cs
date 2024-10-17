using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using static BulletHelper;
using static EnemyHelper;
using static HealthHelper;
using static PlayerAuthoring;
using static PlayerHelper;
using static TriggerHelper;

/// <summary>
/// �ڐG�����ۂ̏���
/// </summary>
static public partial class TriggerJobs
{
    // EnemyDamageTriggerJob��PlayerDamageTriggerJob��Z�߂��Ȃ����낤��
    // �G�ɖ��G���ԈȊO�̗v�f��^���ĘA��Hit���Ȃ��悤�ɏC������
    // Player�����S���ɍ폜�����悤�ɂ���
    // �폜������HP�ł͂Ȃ��ʂ̏ꏊ�ɍ��Ȃ����낤��
    // ���S���̉��o��VFXGraph���쐬���ČĂяo��
    // �͈͂���o���e��G���폜����

    /* �ȉ��̏�����KillData���擾���A�t���O��؂�ւ���
     * 
     * EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
     * Entity myEntity = // ���炩�̕��@�ŃG���e�B�e�B���擾
     * MyComponentAccessor.GetComponentDataFromEntity(myEntity, entityManager);
     * 
     * System��Job�ȊO�̏ꏊ�ŏ�L�̏����𑽗p����̂͏������d���Ȃ�̂ł͂Ȃ���
     * �ꃕ���œZ�߂�entityManager���g������������������y�ʂȂ̂ł͂Ȃ���
     */



    /// <summary>
    /// �ڐG���Ƀ_���[�W���󂯂�
    /// </summary>
    public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
    {
        // �C���X�^���X�̎擾�ɕK�v�ȕϐ�
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;

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
            if (dealDamage.campsType == CampsType.Player) { return; }

            // �_���[�W��^���A�ύX���ꂽ�C���X�^���X�𔽉f����
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // �U�����󂯂��Ώۂ��폜��DestroyableData��L���Ă��Ȃ���ΐ؂�グ��
            if (!destroyableLookup.HasComponent(entityB)) { return; }

            var destroyable = destroyableLookup[entityB];

            // HP�̍폜�t���O��������
            destroyable.isKilled = healthPoint.isKilled;

            // �ύX�𔽉f
            destroyableLookup[entityB] = destroyable;
        }
    }

    /// <summary>
    /// �ڐG���Ƀ_���[�W���󂯂�
    /// </summary>
    public partial struct EnemyDamageTriggerJob : ITriggerEventsJob
    {
        // �C���X�^���X�̎擾�ɕK�v�ȕϐ�
        public ComponentLookup<EnemyHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
        public ComponentLookup<DestroyableData> destroyableLookup;

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
            if (dealDamage.campsType == CampsType.Enemy) { return; }

            // �_���[�W��^���A�ύX���ꂽ�C���X�^���X�𔽉f����
            healthPoint = dealDamage.DealDamage(healthPoint, entityA);
            healthPointLookup[entityB] = healthPoint;

            // �U�����󂯂��Ώۂ��폜��DestroyableData��L���Ă��Ȃ���ΐ؂�グ��
            if (!destroyableLookup.HasComponent(entityB)) { return; }

            var destroyable = destroyableLookup[entityB];

            // HP�̍폜�t���O��������
            destroyable.isKilled = healthPoint.isKilled;

            // �ύX�𔽉f
            destroyableLookup[entityB] = destroyable;
        }
    }
}
