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

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
            var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

            if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

            var dealDamage = dealDamageLookup[entityA];
            var healthPoint = healthPointLookup[entityB];

            dealDamage.DealDamage(healthPoint);
        }
    }
}
