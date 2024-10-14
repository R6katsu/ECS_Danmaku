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
    /// HPを実装
    /// </summary>
    public interface IHealthPoint
    {
        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage">被ダメージ</param>
        public void DamageHP(float damage, double currentTime);

        /// <summary>
        /// HPを回復する
        /// </summary>
        /// <param name="heal">回復量</param>
        public void HealHP(float heal);

        /// <summary>
        /// 倒れる
        /// </summary>
        public void Down();
    }

    /// <summary>
    /// 与ダメージを実装
    /// </summary>
    public interface IDealDamage
    {
        /// <summary>
        /// IHealthPointへダメージを与える
        /// </summary>
        /// <param name="healthPoint">実装されたHP</param>
        /// <returns>変更を適用したインスタンス</returns>
        public T DealDamage<T>(T healthPoint, double currentTime) where T : IHealthPoint;
    }
}