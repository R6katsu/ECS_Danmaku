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
/// HPの補助
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
        public void DamageHP(float damage);

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
        /// <typeparam name="T">IHealthPointを継承した型</typeparam>
        /// <param name="healthPoint">実装されたHP</param>
        /// <returns>変更を適用したインスタンス</returns>
        public T DealDamage<T>(T healthPoint) where T : IHealthPoint;
    }
}