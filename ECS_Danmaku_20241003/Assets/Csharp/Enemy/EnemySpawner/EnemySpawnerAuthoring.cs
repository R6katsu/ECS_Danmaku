using System;
using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
using System.Reflection;
using System.Linq;
using Unity.Mathematics;
using System.Net;
using static EnemyHelper;
#endif

/// <summary>
/// 敵の生成に必要な情報（シングルトン前提）
/// </summary>
public struct EnemySpawnerSingletonData : IComponentData
{
    [Tooltip("生成するPrefabのEntity")]
    public readonly Entity enemyEntity;

    [Tooltip("生成後の移動方向")]
    public readonly DirectionTravelType directionTravelType;

    [Tooltip("生成する線の始点")]
    public readonly Vector3 startPoint;

    [Tooltip("生成する線の終点")]
    public readonly Vector3 endPoint;

    public readonly FixedList4096Bytes<EnemySpawnInfo> enemySpawnInfos;

    /// <summary>
    /// 敵の生成に必要な情報
    /// </summary>
    /// <param name="enemyEntity">生成するPrefabのEntity</param>
    /// <param name="directionTravelType">生成後の移動方向</param>
    /// <param name="startPoint">生成する線の始点</param>
    /// <param name="endPoint">生成する線の終点</param>
    /// <param name="enemySpawnInfos">敵生成情報のList</param>
    public EnemySpawnerSingletonData(
        Entity enemyEntity,
        DirectionTravelType directionTravelType,
        Vector3 startPoint,
        Vector3 endPoint,
        FixedList4096Bytes<EnemySpawnInfo> enemySpawnInfos)
    {
        this.enemyEntity = enemyEntity;
        this.directionTravelType = directionTravelType;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.enemySpawnInfos = enemySpawnInfos;
    }
}

/// <summary>
/// 敵の生成設定
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class EnemySpawnerAuthoring : SingletonMonoBehaviour<EnemySpawnerAuthoring>
{
    // 生成する線の可視化
    void OnDrawGizmos()
    {
        if (_startPoint != null && _endPoint != null)
        {
            // 線の色を設定
            Gizmos.color = Color.red;

            // 始点と終点を結ぶ線を描画
            Gizmos.DrawLine(_startPoint.position, _endPoint.position);
        }
    }

    [SerializeField, Header("生成した敵の移動方向")]
    private DirectionTravelType _directionTravelType = 0;

    [Header("生成する位置の線を書く為の始点と終点を設定する")]
    [SerializeField] protected Transform _startPoint = null;
    [SerializeField] protected Transform _endPoint = null;

    [SerializeField, Header("生成する敵のPrefab配列")]
    private GameObject[] _enemyPrefabs = null;

    [SerializeField]
    private EnemySpawnSettingSO _enemySpawnSettingSO = null;

#if UNITY_EDITOR
    private void OnEnable()
    {
        // 始点がnullだったら作成する
        if (_startPoint == null)
        {
            var startPoint = new GameObject();
            startPoint.name = "startPoint";
            startPoint.transform.parent = transform;
            _startPoint = startPoint.transform;
        }

        // 終点がnullだったら作成する
        if (_endPoint == null)
        {
            var endPoint = new GameObject();
            endPoint.name = "endPoint";
            endPoint.transform.parent = transform;
            _endPoint = endPoint.transform;
        }
    }
#endif

    public class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring src)
        {
            foreach (var enemyPrefab in src._enemyPrefabs)
            {
                var enemy = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                var enemySpawnInfos = new FixedList4096Bytes<EnemySpawnInfo>();
                foreach (var info in src._enemySpawnSettingSO.Info)
                {
                    enemySpawnInfos.Add(info);
                }

                // 敵を生成する為に必要な情報を作成
                var enemySpawnerData = new EnemySpawnerSingletonData
                    (
                        enemy,
                        src._directionTravelType,
                        src._startPoint.position,
                        src._endPoint.position,
                        enemySpawnInfos
                    );

                // 空のEntityを作成し、敵の生成に必要な情報をアタッチする
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                AddComponent(tmpEnemy, enemySpawnerData);
            }
        }
    }
}