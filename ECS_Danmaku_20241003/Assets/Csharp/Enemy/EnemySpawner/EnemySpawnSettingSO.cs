using System;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static EnemySpawnSettingSO;
#endif

/// <summary>
/// 敵生成関連の情報の設定
/// </summary>
[Serializable]
public struct EnemySpawnSettingInfo
{
    [SerializeField, Header("生成までの時間")]
    private float _creationDelay;

    [SerializeField, Header("生成する位置")]
    private float3 _spawnPosition;

    /// <summary>
    /// 生成までの時間
    /// </summary>
    public float CreationDelay => _creationDelay;

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
    // 使いにくい
    // 敵の設定が難しい。要素数が増えると変更が困難になる
    // 敵の出現位置を事前に幾つか設定しておき、レーンという概念で設定する方が良さそう
    // もっと分かりやすく操作性の良い敵の出現設定を作りたい

    [SerializeField]
    private TransformEnemySpawnInfo[] _info = null;

    /// <summary>
    /// 生成する敵の情報配列
    /// </summary>
    public TransformEnemySpawnInfo[] Info => _info;
}
