using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;
using static EnemyHelper;

#if UNITY_EDITOR
using System.Reflection;
using System.Linq;
using Unity.Mathematics;
using System.Net;
#endif

/// <summary>
/// 敵の生成設定
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class EnemySpawnerAuthoring : MonoBehaviour
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

    /// <summary>
    /// 生成した敵の移動方向
    /// </summary>
    public DirectionTravelType MyDirectionTravelType => _directionTravelType;

    /// <summary>
    /// 生成する位置の線の始点
    /// </summary>
    public Transform StartPoint => _startPoint;

    /// <summary>
    /// 生成する位置の線の終点
    /// </summary>
    public Transform EndPoint => _endPoint;

    /// <summary>
    /// 生成する敵のPrefab配列
    /// </summary>
    public GameObject[] EnemyPrefabs => _enemyPrefabs;

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
            foreach (var enemyPrefab in src.EnemyPrefabs)
            {
                var enemy = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                // 敵を生成する為に必要な情報を作成
                var enemySpawnerData = new EnemySpawnerData
                    (
                        enemy,
                        src.MyDirectionTravelType,
                        src.StartPoint.position,
                        src.EndPoint.position
                    );

                // 空のEntityを作成し、敵の生成に必要な情報をアタッチする
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                AddComponent(tmpEnemy, enemySpawnerData);
            }
        }
    }
}