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
    /// <summary>
    /// �ڐG���Ƀ_���[�W���󂯂�
    /// </summary>
    public partial struct PlayerDamageTriggerJob : ITriggerEventsJob
    {
        public ComponentLookup<PlayerHealthPointData> healthPointLookup;
        public ComponentLookup<BulletIDealDamageData> dealDamageLookup;

        public double currentTime;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
            var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

            // entityA��BulletIDealDamageData��L���Ă��Ȃ��B
            // ���邢�́AentityB��PlayerHealthPointData��L���Ă��Ȃ�
            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            // entityB����PlayerHealthPointData���擾
            var healthPoint = healthPointLookup[entityB];

            // �O���e�������Ԃ��疳�G���Ԃ̒������o�߂��Ă��Ȃ�
            if (currentTime - healthPoint.lastHitTime <= healthPoint.isInvincibleTime) { return; }

            // entityA����BulletIDealDamageData���擾
            var dealDamage = dealDamageLookup[entityA];

            healthPointLookup[entityB] = dealDamage.DealDamage(healthPoint, currentTime);
        }
    }
}
