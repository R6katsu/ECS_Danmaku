using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

#if UNITY_EDITOR
using NUnit.Framework;
using System.Security.Cryptography;
using Unity.Core;
#endif

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

    public class EnemyHealthPointDataDic
    {
        // あとで修正する。とりあえずの奴
        // BurstCompile属性を付与している場所から呼び出した場合はエラーになる

        static public Dictionary<int, List<Entity>> entitys = new();
    }
}
