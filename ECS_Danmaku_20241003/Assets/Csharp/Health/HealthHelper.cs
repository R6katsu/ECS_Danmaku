using Unity.Entities;

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HealthHelper;
using static PlayerAuthoring;
#endif

// リファクタリング済み

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
        /// <param name="entity">接触した相手</param>
        /// <param name="frameCount">接触したフレーム</param>
        public void DamageHP(float damage, Entity entity, int frameCount);

        /// <summary>
        /// HPを回復する
        /// </summary>
        /// <param name="heal">回復量</param>
        /// <param name="entity">相手</param>
        public void HealHP(float heal, Entity entity);
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
        /// <param name="entity">自身</param>
        /// <param name="frameCount">接触したフレーム</param>
        /// <returns>変更を適用したインスタンス</returns>
        public T DealDamage<T>(T healthPoint, Entity entity, int frameCount) where T : IHealthPoint;
    }
}