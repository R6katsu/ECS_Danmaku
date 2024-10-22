using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static HealthHelper;
using static HealthPointDataAspect;

/// <summary>
/// HealthPointData���ꊇ��ɂ���
/// </summary>
static public class HealthPointDatas
{
    /// <summary>
    /// Player��IHealthPoint���
    /// </summary>
    public struct PlayerHealthPointData : IComponentData, IHealthPoint
    {
        [Tooltip("�ő�̗�")]
        public readonly float maxHP;

        [Tooltip("���G���Ԃ̒���")]
        public readonly float isInvincibleTime;

        [Tooltip("�����̗�")]
        public float currentHP;

        [Tooltip("���G����")]
        public bool isInvincible;

        [Tooltip("�O���e��������")]
        public double lastHitTime;

        [Tooltip("�폜�t���O")]
        public bool isKilled;

        /// <summary>
        /// �ő�̗�
        /// </summary>
        public float MaxHP => maxHP;

        /// <summary>
        /// �����̗�
        /// </summary>
        public float CurrentHP => currentHP;

        /// <summary>
        /// Player��IHealthPoint���
        /// </summary>
        /// <param name="maxHP">�ő�̗�</param>
        public PlayerHealthPointData(float maxHP, float isInvincibleTime)
        {
            this.maxHP = maxHP;
            this.isInvincibleTime = isInvincibleTime;

            // ������
            currentHP = this.maxHP;
            isInvincible = false;
            lastHitTime = 0.0f;
            isKilled = false;
        }

        // IHealthPoint
        public void DamageHP(float damage, Entity entity)
        {
            var damageHP = HealthPointDataAspect.GetDamage(ASDFGHJKL.PlayerDamage);
            this = damageHP(damage, entity, this);
        }

        // IHealthPoint
        public void HealHP(float heal, Entity entity)
        {
            Debug.Log("HP���񕜂���");
        }

        // IHealthPoint
        public void Down()
        {
            isKilled = true;
        }
    }

    /// <summary>
    ///Enemy��IHealthPoint���
    /// </summary>
    public struct EnemyHealthPointData : IComponentData, IHealthPoint
    {
        [Tooltip("�ő�̗�")]
        public readonly float maxHP;

        [Tooltip("���G���Ԃ̒���")]
        public readonly float isInvincibleTime;

        [Tooltip("�����̗�")]
        private float _currentHP;

        [Tooltip("���G����")]
        public bool isInvincible;

        [Tooltip("�O���e��������")]
        public double lastHitTime;

        [Tooltip("�폜�t���O")]
        public bool isKilled;

        [Tooltip("���݂̎��ʔԍ�")]
        static private int currentNumber;

        [Tooltip("���g�̎��ʔԍ�")]
        private int _myNumber;

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
        public EnemyHealthPointData(float maxHP, float isInvincibleTime)
        {
            this.maxHP = maxHP;
            this.isInvincibleTime = isInvincibleTime;

            // ������
            _currentHP = this.maxHP;
            isInvincible = false;
            lastHitTime = 0.0f;
            isKilled = false;
            currentNumber++;
            _myNumber = 0;
        }

        // IHealthPoint
        public void DamageHP(float damage, Entity entity)
        {
            Debug.Log("DamageHP");

            // �܂�Key���܂܂�Ă��Ȃ���Ύ��g��ǉ�����
            if (!EnemyHealthPointDataDic.entitys.ContainsKey(MyNumber))
            {
                EnemyHealthPointDataDic.entitys.Add(MyNumber, new());
            }

            // ����List�Ɋ܂܂�Ă���Entity��������؂�グ��
            if (EnemyHealthPointDataDic.entitys[MyNumber].Contains(entity)) { return; }

            // �܂��ڐG���Ă��Ȃ�Entity�������̂�List�ɒǉ�����
            EnemyHealthPointDataDic.entitys[MyNumber].Add(entity);

            Debug.Log("�����I�ȃR�[�h�ׁ̈A��Œ���");
            Debug.Log("�G�Ƀ_���[�W��^����");
            _currentHP -= damage;
            lastHitTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;

            // HP�� 0�ȉ��Ȃ�|���
            if (_currentHP <= 0)
                Down();
        }

        // IHealthPoint
        public void HealHP(float heal, Entity entity)
        {
            Debug.Log("HP���񕜂���");
        }

        // IHealthPoint
        public void Down()
        {
            isKilled = true;
        }
    }
}
