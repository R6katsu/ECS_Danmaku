using System;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using static UnityEditor.Progress;
using Unity.Entities.UniversalDelegates;
using System.Collections;
using System.Collections.Generic;
using static EnemySpawnPatternSettingSO;
#endif

// リファクタリング済み

/// <summary>
/// 敵生成設定の情報の設定
/// </summary>
[Serializable]
public struct EnemySpawnInfo
{
    // 生成する敵Entityの名称
    [SerializeField]
    private EnemyName _enemyName;

    // 生成までの時間
    [SerializeField, Min(0)]
    private float _creationDelay;

    // 生成位置の種類
    [SerializeField]
    private SpawnPointType _spawnPointType;

    /// <summary>
    /// 生成する敵Entityの名称
    /// </summary>
    public EnemyName MyEnemyName => _enemyName;

    /// <summary>
    /// 生成までの時間
    /// </summary>
    public float CreationDelay => _creationDelay;

    /// <summary>
    /// 生成位置の種類
    /// </summary>
    public SpawnPointType SpawnPointType => _spawnPointType;
}

/// <summary>
/// 敵生成関連の情報の配列の配列
/// </summary>
[Serializable]
public struct EnemySpawnPatternArray
{
    [Tooltip("敵生成関連の情報の配列の配列")]
    public EnemySpawnInfo[] infos;
}

/// <summary>
/// 敵の生成パターンの設定
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnPatternSettingSO", menuName = "ScriptableObject/EnemySpawnPatternSettingSO")]
public class EnemySpawnPatternSettingSO : ScriptableObject
{
    [SerializeField, Header("ボス敵Prefab")]
    private Transform _bossEnemyPrefab = null;

    [SerializeField, Header("ボス生成までのカウントダウン")]
    private int _countdownBossSpawn;

    [SerializeField, Header("敵生成関連の情報の配列の配列の配列")]
    private EnemySpawnPatternArray[] _patternArrays = null;

    /// <summary>
    /// ボス敵Prefab
    /// </summary>
    public Transform BossEnemyPrefab => _bossEnemyPrefab;

    /// <summary>
    /// ボス生成までのカウントダウン
    /// </summary>
    public int CountdownBossSpawn => _countdownBossSpawn;

    /// <summary>
    /// 敵生成関連の情報の配列の配列の配列
    /// </summary>
    public EnemySpawnPatternArray[] PatternArrays => _patternArrays;
}
