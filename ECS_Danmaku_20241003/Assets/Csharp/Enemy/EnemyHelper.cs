using Unity.Core;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

/// <summary>
/// 敵の補助
/// </summary>
static public class EnemyHelper
{
    /// <summary>
    /// 敵
    /// </summary>
    public struct EnemyTag : IComponentData{ }

    /// <summary>
    /// 敵の生成に必要な情報
    /// </summary>
    public struct EnemySpawnerData : IComponentData
    {
        [Tooltip("生成するPrefabのEntity")]
        public readonly Entity enemyEntity;

        [Tooltip("生成後の移動方向")]
        public readonly DirectionTravelType directionTravelType;
        
        [Tooltip("生成する線の始点")]
        public readonly Vector3 startPoint;

        [Tooltip("生成する線の終点")]
        public readonly Vector3 endPoint;

        /// <summary>
        /// 敵の生成に必要な情報
        /// </summary>
        /// <param name="enemyEntity">生成するPrefabのEntity</param>
        /// <param name="directionTravelType">生成後の移動方向</param>
        /// <param name="startPoint">生成する線の始点</param>
        /// <param name="endPoint">生成する線の終点</param>
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
    ///EnemyのIHealthPoint情報
    /// </summary>
    public struct EnemyHealthPointData : IComponentData, IHealthPoint
    {
        [Tooltip("最大体力")]
        public readonly float maxHP;

        [Tooltip("無敵時間の長さ")]
        public readonly float isInvincibleTime;     // 敵には無敵時間は必要ない、ただ連続して同じ弾が当たらないようにする

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
        }

        // IHealthPoint
        public void DamageHP(float damage)
        {
            Debug.Log("試験的なコードの為、後で直す");
            Debug.Log("敵にダメージを与えた");
            _currentHP -= damage;
            lastHitTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;

            // HPが 0以下なら倒れる
            if (_currentHP <= 0)
                Down();
        }

        // IHealthPoint
        public void HealHP(float heal)
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
