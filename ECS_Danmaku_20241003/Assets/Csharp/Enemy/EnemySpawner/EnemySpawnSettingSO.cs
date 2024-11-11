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
/// “G¶¬ŠÖ˜A‚Ìî•ñ‚Ìİ’è
/// </summary>
[Serializable]
public struct EnemySpawnSettingInfo
{
    [SerializeField, Header("¶¬‚Ü‚Å‚ÌŠÔ")]
    private float _creationDelay;

    [SerializeField, Header("¶¬‚·‚éˆÊ’u")]
    private float3 _spawnPosition;

    /// <summary>
    /// ¶¬‚Ü‚Å‚ÌŠÔ
    /// </summary>
    public float CreationDelay => _creationDelay;

    /// <summary>
    /// ¶¬‚·‚éˆÊ’u
    /// </summary>
    public float3 SpawnPosition => _spawnPosition;
}

/// <summary>
/// ¶¬‚·‚é“G‚Ìİ’è
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnSettingSO", menuName = "ScriptableObject/EnemySpawnSettingSO")]
public class EnemySpawnSettingSO : ScriptableObject
{
    [SerializeField]
    private TransformEnemySpawnInfo[] _info = null;

    /// <summary>
    /// ¶¬‚·‚é“G‚Ìî•ñ”z—ñ
    /// </summary>
    public TransformEnemySpawnInfo[] Info => _info;
}
