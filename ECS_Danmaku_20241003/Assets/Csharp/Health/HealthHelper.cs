#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static PlayerAuthoring;
#endif

/// <summary>
/// HP�̕⏕
/// </summary>
static public class HealthHelper
{
    /// <summary>
    /// HP������
    /// </summary>
    public interface IHealthPoint
    {
        /// <summary>
        /// �_���[�W���󂯂�
        /// </summary>
        /// <param name="damage">��_���[�W</param>
        /// <param name="entity">����</param>
        public void DamageHP(float damage, Entity entity);

        /// <summary>
        /// HP���񕜂���
        /// </summary>
        /// <param name="heal">�񕜗�</param>
        /// <param name="entity">����</param>
        public void HealHP(float heal, Entity entity);

        /// <summary>
        /// �|���
        /// </summary>
        public void Down();
    }

    /// <summary>
    /// �^�_���[�W������
    /// </summary>
    public interface IDealDamage
    {
        /// <summary>
        /// IHealthPoint�փ_���[�W��^����
        /// </summary>
        /// <typeparam name="T">IHealthPoint���p�������^</typeparam>
        /// <param name="healthPoint">�������ꂽHP</param>
        /// <param name="entity">���g</param>
        /// <returns>�ύX��K�p�����C���X�^���X</returns>
        public T DealDamage<T>(T healthPoint, Entity entity) where T : IHealthPoint;
    }
}