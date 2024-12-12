using Unity.Entities;
using UnityEngine;
using static HealthHelper;

#if UNITY_EDITOR
using System.Collections.Generic;
using static HealthPointDatas;
using static EnemyHelper;
using Unity.Collections;
using System.Collections;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// HealthPointData���ꊇ��ɂ���
/// </summary>
static public class HealthPointDatas
{
    /// <summary>
    /// ���݂�IHealthPoint���
    /// </summary>
    public struct HealthPointData : IComponentData, IHealthPoint
    {
        [Tooltip("�ő�̗�")]
        public readonly float maxHP;

        [Tooltip("��e���̌��ʉ��ԍ�")]
        public readonly int hitSENumber;

        [Tooltip("���S���̌��ʉ��ԍ�")]
        public readonly int killedSENumber;

        [Tooltip("�����̗�")]
        private float _currentHP;

        [Tooltip("���݂̎��ʔԍ�")]
        static private int currentNumber;

        [Tooltip("���g�̎��ʔԍ�")]
        private int _myNumber;

        [Tooltip("�Ō�ɐڐG��������")]
        private Entity _lastHitEntity;

        /// <summary>
        /// �ő�̗�
        /// </summary>
        public float MaxHP => maxHP;

        /// <summary>
        /// �����̗�
        /// </summary>
        public float CurrentHP => _currentHP;

        /// <summary>
        /// ���g�̎��ʔԍ�
        /// </summary>
        private int MyNumber
        {
            get
            {
                // ������Ԃ̂܂܂�����
                if (_myNumber == 0)
                {
                    // ���ʔԍ����X�V���A�擾����
                    currentNumber++;
                    _myNumber = currentNumber;
                }

                return _myNumber;
            }
        }

        /// <summary>
        /// Enemy��IHealthPoint���
        /// </summary>
        /// <param name="maxHP">�ő�̗�</param>
        /// <param name="hitSENumber">��e���̌��ʉ��ԍ�</param>
        /// <param name="killedSENumber">���S���̌��ʉ��ԍ�</param>
        public HealthPointData(float maxHP, int hitSENumber, int killedSENumber)
        {
            this.maxHP = maxHP;
            this.hitSENumber = hitSENumber;
            this.killedSENumber = killedSENumber;

            // ������
            _currentHP = this.maxHP;
            currentNumber++;
            _myNumber = 0;
            _lastHitEntity = Entity.Null;
        }

        // IHealthPoint
        public void DamageHP(float damage, Entity entity)
        {
            // �O��Ɠ������肾����
            if (_lastHitEntity == entity) { return; }
            
            // �Ō�ɐڐG����������X�V
            _lastHitEntity = entity;

            _currentHP -= damage;
        }

        // IHealthPoint
        public void HealHP(float heal, Entity entity)
        {
            _currentHP += heal;
        }
    }
}
