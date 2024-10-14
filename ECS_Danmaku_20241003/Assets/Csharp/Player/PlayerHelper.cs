using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;

static public class PlayerHelper
{
    /// <summary>
    /// Player
    /// </summary>
    public struct PlayerTag : IComponentData { }

    /// <summary>
    /// Player��IHealthPoint���
    /// </summary>
    public struct PlayerHealthPointData : IComponentData, IHealthPoint
    {

        public readonly float maxHP;

        [Tooltip("���G���Ԃ̒���")]
        public readonly float isInvincibleTime;

        private float _currentHP;

        [Tooltip("���G����")]
        public bool isInvincible;

        [Tooltip("�O���e��������")]
        public double lastHitTime;

        /// <summary>
        /// �ő�̗�
        /// </summary>
        public float MaxHP => maxHP;

        /// <summary>
        /// �c�ʑ̗�
        /// </summary>
        public float CurrentHP => _currentHP;

        /// <summary>
        /// Player��IHealthPoint���
        /// </summary>
        /// <param name="maxHP">�ő�̗�</param>
        public PlayerHealthPointData(float maxHP, float isInvincibleTime)
        {
            this.maxHP = maxHP;
            this.isInvincibleTime = isInvincibleTime;

            _currentHP = this.maxHP;
            isInvincible = false;
            lastHitTime = 0.0f;
        }

        // IHealthPoint
        public void DamageHP(float damage, double currentTime)
        {
            _currentHP -= damage;
            lastHitTime = currentTime;
            Debug.Log($"{damage}�_���[�W���󂯂��B�c��HP{_currentHP}");

            // HP�� 0�ȉ��Ȃ�|���
            if (_currentHP <= 0)
                Down();
        }

        // IHealthPoint
        public void HealHP(float heal)
        {
            // HP�ɂ��āAData�����x�ɍ�蒼�����ƂɂȂ��Ă��܂�
            Debug.Log("HP���񕜂���");
        }

        // IHealthPoint
        public void Down()
        {
            Debug.Log("�|���");
        }
    }
}
