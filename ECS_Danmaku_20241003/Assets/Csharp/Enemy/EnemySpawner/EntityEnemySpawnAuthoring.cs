using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

#if UNITY_EDITOR
using System;
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

// リファクタリング済み

/// <summary>
/// 生成位置の種類
/// </summary>
public enum SpawnPointType : byte
{
    [Tooltip("左")] Left,
    [Tooltip("中心と左の間")] LeftSpacer,
    [Tooltip("中心")] Center,
    [Tooltip("中心と右の間")] RightSpacer,
    [Tooltip("右")] Right
}

/// <summary>
/// 敵生成設定の情報の配列
/// </summary>
public struct EnemySpawnPattern
{
    [Tooltip("敵生成設定の情報の配列")]
    public readonly FixedList64Bytes<EnemySpawnInfo> enemySpawnInfos;

    /// <summary>
    /// 敵生成関連の情報の配列
    /// </summary>
    /// <param name="enemySpawnInfos">配列の要素をFixedList64Bytesに代入</param>
    public EnemySpawnPattern(EnemySpawnInfo[] enemySpawnInfos)
    {
        // 初期割り当て
        this.enemySpawnInfos = new();

        // 配列の要素をFixedList64Bytesに代入
        foreach (var enemySpawnInfo in enemySpawnInfos)
        {
            this.enemySpawnInfos.Add(enemySpawnInfo);
        }
    }
}

/// <summary>
/// 敵生成設定の情報の配列
/// </summary>
public struct EnemySpawnPatternArraySingletonData : IComponentData
{
    [Tooltip("敵生成設定の情報の配列")]
    public FixedList4096Bytes<EnemySpawnPattern> enemySpawnPatterns;

    [Tooltip("ボス敵Entity")]
    public readonly Entity bossEnemyEntity;

    [Tooltip("ボス生成までのカウントダウン")]
    public readonly int countdownBossSpawn;

    /// <summary>
    /// 敵生成関連の情報の配列
    /// </summary>
    /// <param name="enemySpawnPatternArrays">配列の要素をFixedList4096Bytesに代入</param>
    /// <param name="bossEnemyEntity">ボス敵Entity</param>
    /// <param name="countdownBossSpawn">ボス生成までのカウントダウン</param>
    public EnemySpawnPatternArraySingletonData
    (
        EnemySpawnPatternArray[] enemySpawnPatternArrays,
        Entity bossEnemyEntity, 
        int countdownBossSpawn)
    {
        this.bossEnemyEntity = bossEnemyEntity;
        this.countdownBossSpawn = countdownBossSpawn;

        // 初期割り当て
        this.enemySpawnPatterns = new();

        if (enemySpawnPatternArrays == null)
        {
#if UNITY_EDITOR
            Debug.LogError("enemySpawnInfoArrayDatasがnull");
#endif
            return;
        }

        for (int i = 0; i < enemySpawnPatternArrays.Length; i++)
        {
            var currentArraysLength = enemySpawnPatternArrays[i].infos.Length;
            var enemySpawnInfo = new EnemySpawnInfo[currentArraysLength];

            for (int j = 0; j < currentArraysLength; j++)
            {
                enemySpawnInfo[j] = enemySpawnPatternArrays[i].infos[j];
            }

            var enemySpawnPattern = new EnemySpawnPattern(enemySpawnInfo);
            enemySpawnPatterns.Add(enemySpawnPattern);
        }
    }
}

/// <summary>
/// 生成位置の情報（シングルトン前提）
/// </summary>
public struct SpawnPointSingletonData : IComponentData
{
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

        // 左右以外の生成位置を計算
        centerPoint = (leftPoint + rightPoint).Halve();
        leftSpacerPoint = (leftPoint + centerPoint).Halve();
        rightSpacerPoint = (rightPoint + centerPoint).Halve();
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
/// 敵の生成に必要な設定
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

            // 情報を保持するだけなのでNone
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, spawnPointSingletonData);

            var bossEntity = GetEntity(src._enemySpawnSettingSO.BossEnemyPrefab, TransformUsageFlags.None);

            var enemySpawnPatternArraySingletonData = new EnemySpawnPatternArraySingletonData
            (
                src._enemySpawnSettingSO.PatternArrays, 
                bossEntity, 
                src._enemySpawnSettingSO.CountdownBossSpawn
            );

            AddComponent(entity, enemySpawnPatternArraySingletonData);
        }
    }
}