using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

/// <summary>
/// �G�̕⏕
/// </summary>
static public class EnemyHelper
{
    /// <summary>
    /// �G
    /// </summary>
    public struct EnemyTag : IComponentData{ }

    /// <summary>
    /// �G�̐����ɕK�v�ȏ��
    /// </summary>
    public struct EnemySpawnerData : IComponentData
    {
        [Tooltip("��������Prefab��Entity")]
        public readonly Entity enemyEntity;

        [Tooltip("������̈ړ�����")]
        public readonly DirectionTravelType directionTravelType;
        
        [Tooltip("����������̎n�_")]
        public readonly Vector3 startPoint;

        [Tooltip("����������̏I�_")]
        public readonly Vector3 endPoint;

        /// <summary>
        /// �G�̐����ɕK�v�ȏ��
        /// </summary>
        /// <param name="enemyEntity">��������Prefab��Entity</param>
        /// <param name="directionTravelType">������̈ړ�����</param>
        /// <param name="startPoint">����������̎n�_</param>
        /// <param name="endPoint">����������̏I�_</param>
        public EnemySpawnerData(
            Entity enemyEntity,
            DirectionTravelType directionTravelType,
            Vector3 startPoint, 
            Vector3 endPoint)
        {
            this.enemyEntity = enemyEntity;
            this.directionTravelType = directionTravelType;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
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

    public class EnemyHealthPointDataDic
    {
        // ���ƂŏC������B�Ƃ肠�����̓z

        static public Dictionary<int, List<Entity>> entitys = new();
    }
}
