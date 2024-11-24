using System;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

#if UNITY_EDITOR
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using static MoveHelper;
using System.Security.Cryptography;
using Unity.Entities.UniversalDelegates;
using System.Reflection;
using System.Linq;
using System.Net;
using static EnemyHelper;
using static UnityEngine.EventSystems.EventTrigger;
#endif

/// <summary>
/// 敵生成設定の情報の配列
/// </summary>
public struct EnemySpawnPattern
{
    [Tooltip("敵生成設定の情報の配列")]
    public readonly FixedList64Bytes<EnemySpawnInfo> infos;

    /// <summary>
    /// 敵生成関連の情報の配列
    /// </summary>
    /// <param name="enemySpawnSettingDatas">配列の要素をFixedList64Bytesに代入</param>
    public EnemySpawnPattern(EnemySpawnInfo[] enemySpawnSettingDatas)
    {
        // 初期割り当て
        this.infos = new();

        // 配列の要素をFixedList64Bytesに代入
        foreach (var enemySpawnSettingData in enemySpawnSettingDatas)
        {
            this.infos.Add(enemySpawnSettingData);
        }
    }
}

/// <summary>
/// 敵生成設定の情報の配列
/// </summary>
public struct EnemySpawnPatternArraySingletonData : IComponentData
{
    [Tooltip("敵生成設定の情報の配列")]
    public FixedList4096Bytes<EnemySpawnPattern> infos;

    [Tooltip("ボス敵Entity")]
    public readonly Entity bossEnemyEntity;

    /// <summary>
    /// 敵生成関連の情報の配列
    /// </summary>
    /// <param name="enemySpawnInfoArrayDatas">配列の要素をFixedList4096Bytesに代入</param>
    public EnemySpawnPatternArraySingletonData(EnemySpawnPatternArray[] enemySpawnInfoArrayDatas, Entity bossEnemyEntity)
    {
        this.bossEnemyEntity = bossEnemyEntity;

        // 初期割り当て
        this.infos = new();
        var tempInfos = new FixedList4096Bytes<EnemySpawnPattern>();

        if (enemySpawnInfoArrayDatas == null)
        {
            Debug.LogError("enemySpawnInfoArrayDatasがnull！");
            return;
        }

        for (int i = 0; i < enemySpawnInfoArrayDatas.Length; i++)
        {
            //var jfoa = new EnemySpawnInfo[enemySpawnInfoArrayDatas.Length];
            var jfoa = new EnemySpawnInfo[enemySpawnInfoArrayDatas[i].arrays.Length];

            for (int j = 0; j < enemySpawnInfoArrayDatas[i].arrays.Length; j++)
            {
                jfoa[j] = enemySpawnInfoArrayDatas[i].arrays[j];
            }

            var koda = new EnemySpawnPattern(jfoa);
            tempInfos.Add(koda);
        }

        // 配列の要素をFixedList4096Bytesに代入
        this.infos = tempInfos;
    }
}

/// <summary>
/// 生成位置の情報（シングルトン前提）
/// </summary>
public struct SpawnPointSingletonData : IComponentData
{
    /// <summary>
    /// 生成位置の種類
    /// </summary>
    public enum SpawnPointType
    {
        [Tooltip("左")] Left,
        [Tooltip("中心と左の間")] LeftSpacer,
        [Tooltip("中心")] Center,
        [Tooltip("中心と右の間")] RightSpacer,
        [Tooltip("右")] Right
    }

    [Tooltip("左の生成位置")]
    public readonly float3 leftPoint;

    [Tooltip("中心と左の間の生成位置")]
    public readonly float3 leftSpacerPoint;

    [Tooltip("中心の生成位置")]
    public readonly float3 centerPoint;

    [Tooltip("中心と右の間の生成位置")]
    public readonly float3 rightSpacerPoint;

    [Tooltip("右の生成位置")]
    public readonly float3 rightPoint;

    /// <summary>
    /// 生成位置
    /// </summary>
    /// <param name="leftPoint">左の生成位置</param>
    /// <param name="rightPoint">右の生成位置</param>
    public SpawnPointSingletonData(float3 leftPoint, float3 rightPoint)
    {
        this.leftPoint = leftPoint;
        this.rightPoint = rightPoint;

        // /2を定義
        float averageFactor = 2.0f;

        // 左右以外の生成位置を計算
        centerPoint = (leftPoint + rightPoint) / averageFactor;
        leftSpacerPoint = (leftPoint + centerPoint) / averageFactor;
        rightSpacerPoint = (rightPoint + centerPoint) / averageFactor;
    }

    /// <summary>
    /// SpawnPointTypeに対応する生成位置を返す
    /// </summary>
    /// <returns>SpawnPointTypeに対応する生成位置</returns>
    public float3? GetSpawnPoint(SpawnPointType spawnPointType)
    {
        switch (spawnPointType)
        {
            case SpawnPointType.Left:
                return leftPoint;

            case SpawnPointType.LeftSpacer:
                return leftSpacerPoint;

            case SpawnPointType.Center:
                return centerPoint;

            case SpawnPointType.RightSpacer:
                return rightSpacerPoint;

            case SpawnPointType.Right:
                return rightPoint;

            default:
                return null;
        }
    }
}

/// <summary>
/// 敵の生成設定
/// </summary>
public class EntityEnemySpawnAuthoring : SingletonMonoBehaviour<EntityEnemySpawnAuthoring>
{
    [SerializeField]
    private EnemySpawnPatternSettingSO _enemySpawnSettingSO = null;

    [SerializeField, Header("左の生成位置")]
    private float3 _leftPoint = float3.zero;

    [SerializeField, Header("右の生成位置")]
    private float3 _rightPoint = float3.zero;

    public class Baker : Baker<EntityEnemySpawnAuthoring>
    {
        public override void Bake(EntityEnemySpawnAuthoring src)
        {
            // 生成位置の情報を保持するシングルトンを作成
            var spawnPointSingletonData = new SpawnPointSingletonData
                (
                    src._leftPoint,
                    src._rightPoint
                );

            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, spawnPointSingletonData);

            var bossEntity = GetEntity(src._enemySpawnSettingSO.BossEnemyPrefab, TransformUsageFlags.None);

            // 敵生成設定の配列の配列
            var aaaa = new EnemySpawnPatternArraySingletonData(src._enemySpawnSettingSO.Patterns, bossEntity);
            AddComponent(entity, aaaa);

            Debug.Log("動くかの確認段階。リファクタリング必須");
        }
    }
}