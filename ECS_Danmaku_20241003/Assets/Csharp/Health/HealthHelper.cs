using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static PlayerAuthoring;

#if false
using System.Collections.ObjectModel;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
using System.Reflection;
using UnityEditor;
using Unity.VisualScripting;
using static System.Collections.Specialized.BitVector32;
#endif

/// <summary>
/// HP
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
        public void DamageHP(float damage);

        /// <summary>
        /// HP���񕜂���
        /// </summary>
        /// <param name="heal">�񕜗�</param>
        public void HealHP(float heal);
    }

    /// <summary>
    /// �^�_���[�W������
    /// </summary>
    public interface IDealDamage
    {
        /// <summary>
        /// IHealthPoint�փ_���[�W��^����
        /// </summary>
        /// <param name="healthPoint">�������ꂽHP</param>
        public void DealDamage(IHealthPoint healthPoint);
    }
}