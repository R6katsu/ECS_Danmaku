using System;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;
using static EnemySpawnSettingSO;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// 生成する敵の情報
/// </summary>
[Serializable]
public struct EnemySpawnInfo
{
    [SerializeField, Header("生成までの時間")]
    private float _creationDelay;

    [SerializeField, Header("生成する敵の名称")]
    private EnemyName _enemyName;

    [SerializeField, Header("生成する位置")]
    private float3 _spawnPosition;

    /// <summary>
    /// 生成までの時間
    /// </summary>
    public float CreationDelay => _creationDelay;

    /// <summary>
    /// 生成する敵の名称
    /// </summary>
    public EnemyName EnemyName => _enemyName;
    
    /// <summary>
    /// 生成する位置
    /// </summary>
    public float3 SpawnPosition => _spawnPosition;
}

/// <summary>
/// 生成する敵の設定
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnSettingSO", menuName = "ScriptableObject/EnemySpawnSettingSO")]
public class EnemySpawnSettingSO : ScriptableObject
{
    [SerializeField]
    private EnemySpawnInfo[] _info = null;

    /// <summary>
    /// 生成する敵の情報配列
    /// </summary>
    public EnemySpawnInfo[] Info => _info;

    // まず、初めに生成までの秒数が早い順に並び替えた配列を作成する
    // 配列の順番に生成していく。Info.CreationDelay秒を待機してから次の生成をする
    // SystemのUpdate内で次の生成時間まで待機する処理を書く。そうでなければ切り上げる

    // 敵生成のSystemはGameManager側で制御したい
    // ゲーム開始前と終了後は敵を生成しない
    // System自体にboolを持たせてGameManagerから制御する？
}
