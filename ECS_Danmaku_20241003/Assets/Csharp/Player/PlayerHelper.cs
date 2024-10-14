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
    /// PlayerのIHealthPoint情報
    /// </summary>
    public struct PlayerHealthPointData : IComponentData, IHealthPoint
    {

        public readonly float maxHP;

        [Tooltip("無敵時間の長さ")]
        public readonly float isInvincibleTime;

        private float _currentHP;

        [Tooltip("無敵中か")]
        public bool isInvincible;

        [Tooltip("前回被弾した時間")]
        public double lastHitTime;

        /// <summary>
        /// 最大体力
        /// </summary>
        public float MaxHP => maxHP;

        /// <summary>
        /// 残量体力
        /// </summary>
        public float CurrentHP => _currentHP;

        /// <summary>
        /// PlayerのIHealthPoint情報
        /// </summary>
        /// <param name="maxHP">最大体力</param>
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
            Debug.Log($"{damage}ダメージを受けた。残りHP{_currentHP}");

            // HPが 0以下なら倒れる
            if (_currentHP <= 0)
                Down();
        }

        // IHealthPoint
        public void HealHP(float heal)
        {
            // HPについて、Dataを作る度に作り直すことになってしまう
            Debug.Log("HPを回復する");
        }

        // IHealthPoint
        public void Down()
        {
            Debug.Log("倒れる");
        }
    }
}
