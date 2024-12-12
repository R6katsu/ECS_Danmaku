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

// リファクタリング済み

/// <summary>
/// HealthPointDataを一括りにする
/// </summary>
static public class HealthPointDatas
{
    /// <summary>
    /// 現在のIHealthPoint情報
    /// </summary>
    public struct HealthPointData : IComponentData, IHealthPoint
    {
        [Tooltip("最大体力")]
        public readonly float maxHP;

        [Tooltip("被弾時の効果音番号")]
        public readonly int hitSENumber;

        [Tooltip("死亡時の効果音番号")]
        public readonly int killedSENumber;

        [Tooltip("現存体力")]
        private float _currentHP;

        [Tooltip("現在の識別番号")]
        static private int currentNumber;

        [Tooltip("自身の識別番号")]
        private int _myNumber;

        [Tooltip("最後に接触した相手")]
        private Entity _lastHitEntity;

        /// <summary>
        /// 最大体力
        /// </summary>
        public float MaxHP => maxHP;

        /// <summary>
        /// 現存体力
        /// </summary>
        public float CurrentHP => _currentHP;

        /// <summary>
        /// 自身の識別番号
        /// </summary>
        private int MyNumber
        {
            get
            {
                // 初期状態のままだった
                if (_myNumber == 0)
                {
                    // 識別番号を更新し、取得する
                    currentNumber++;
                    _myNumber = currentNumber;
                }

                return _myNumber;
            }
        }

        /// <summary>
        /// EnemyのIHealthPoint情報
        /// </summary>
        /// <param name="maxHP">最大体力</param>
        /// <param name="hitSENumber">被弾時の効果音番号</param>
        /// <param name="killedSENumber">死亡時の効果音番号</param>
        public HealthPointData(float maxHP, int hitSENumber, int killedSENumber)
        {
            this.maxHP = maxHP;
            this.hitSENumber = hitSENumber;
            this.killedSENumber = killedSENumber;

            // 初期化
            _currentHP = this.maxHP;
            currentNumber++;
            _myNumber = 0;
            _lastHitEntity = Entity.Null;
        }

        // IHealthPoint
        public void DamageHP(float damage, Entity entity)
        {
            // 前回と同じ相手だった
            if (_lastHitEntity == entity) { return; }
            
            // 最後に接触した相手を更新
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
