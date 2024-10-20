using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EntityCampsHelper;
using static HealthHelper;

#if UNITY_EDITOR
using static MoveHelper;
using static UnityEngine.EventSystems.EventTrigger;
#endif

/// <summary>
/// �e�̕⏕
/// </summary>
static public class BulletHelper
{
    /// <summary>
    /// �e
    /// </summary>
    public struct BulletTag : IComponentData { }

    /// <summary>
    /// �c��ђʉ񐔂̏��
    /// </summary>
    public struct RemainingPierceCountData : IComponentData
    {
        public int remainingPierceCount;

        /// <summary>
        /// �c��ђʉ񐔂̏��
        /// </summary>
        /// <param name="remainingPierceCount">�c��ђʉ�</param>
        public RemainingPierceCountData(int remainingPierceCount)
        {
            this.remainingPierceCount = remainingPierceCount;
        }
    }

    /// <summary>
    /// �e��IDealDamage���
    /// </summary>
    public struct BulletIDealDamageData : IComponentData, IDealDamage
    {
        public readonly float damageAmount;
        public readonly EntityCampsType campsType;

        /// <summary>
        /// �e��IDealDamage���
        /// </summary>
        /// <param name="damageAmount">�_���[�W��</param>
        public BulletIDealDamageData(float damageAmount, EntityCampsType campsType)
        {
            this.damageAmount = damageAmount;
            this.campsType = campsType;
        }

        // IDealDamage
        public T DealDamage<T>(T healthPoint, Entity entity) where T : IHealthPoint
        {
            healthPoint.DamageHP(damageAmount, entity);
            return healthPoint;
        }
    }
}
