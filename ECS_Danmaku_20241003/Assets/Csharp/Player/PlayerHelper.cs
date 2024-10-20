using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;

/// <summary>
/// PLの補助
/// </summary>
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
        [Tooltip("最大体力")]
        public readonly float maxHP;

        [Tooltip("無敵時間の長さ")]
        public readonly float isInvincibleTime;

        [Tooltip("現存体力")]
        private float _currentHP;

        [Tooltip("無敵中か")]
        public bool isInvincible;

        [Tooltip("前回被弾した時間")]
        public double lastHitTime;

        [Tooltip("削除フラグ")]
        public bool isKilled;

        /// <summary>
        /// 最大体力
        /// </summary>
        public float MaxHP => maxHP;

        /// <summary>
        /// 現存体力
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

            // 初期化
            _currentHP = this.maxHP;
            isInvincible = false;
            lastHitTime = 0.0f;
            isKilled = false;
        }

        // IHealthPoint
        public void DamageHP(float damage, Entity entity)
        {
            Debug.Log("試験的なコードの為、後で直す");
            _currentHP -= damage;
            lastHitTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;

            Debug.Log($"{damage}ダメージを受けた。残りHP{_currentHP}");

            // HPが 0以下なら倒れる
            if (_currentHP <= 0)
                Down();
        }

        // IHealthPoint
        public void HealHP(float heal, Entity entity)
        {
            Debug.Log("HPを回復する");
        }

        // IHealthPoint
        public void Down()
        {
            isKilled = true;
        }
    }
}
