using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static HealthHelper;
using static HealthPointDataAspect;

/// <summary>
/// HealthPointDataを一括りにする
/// </summary>
static public class HealthPointDatas
{
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
        public float currentHP;

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
        public float CurrentHP => currentHP;

        /// <summary>
        /// PlayerのIHealthPoint情報
        /// </summary>
        /// <param name="maxHP">最大体力</param>
        public PlayerHealthPointData(float maxHP, float isInvincibleTime)
        {
            this.maxHP = maxHP;
            this.isInvincibleTime = isInvincibleTime;

            // 初期化
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
            Debug.Log("HPを回復する");
        }

        // IHealthPoint
        public void Down()
        {
            isKilled = true;
        }
    }

    /// <summary>
    ///EnemyのIHealthPoint情報
    /// </summary>
    public struct EnemyHealthPointData : IComponentData, IHealthPoint
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

        [Tooltip("現在の識別番号")]
        static private int currentNumber;

        [Tooltip("自身の識別番号")]
        private int _myNumber;

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
        public EnemyHealthPointData(float maxHP, float isInvincibleTime)
        {
            this.maxHP = maxHP;
            this.isInvincibleTime = isInvincibleTime;

            // 初期化
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

            // まだKeyが含まれていなければ自身を追加する
            if (!EnemyHealthPointDataDic.entitys.ContainsKey(MyNumber))
            {
                EnemyHealthPointDataDic.entitys.Add(MyNumber, new());
            }

            // 既にListに含まれているEntityだったら切り上げる
            if (EnemyHealthPointDataDic.entitys[MyNumber].Contains(entity)) { return; }

            // まだ接触していないEntityだったのでListに追加する
            EnemyHealthPointDataDic.entitys[MyNumber].Add(entity);

            Debug.Log("試験的なコードの為、後で直す");
            Debug.Log("敵にダメージを与えた");
            _currentHP -= damage;
            lastHitTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;

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
