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

/// <summary>
/// �G�����ݒ�̏��̐ݒ�
/// </summary>
[Serializable]
public struct EnemySpawnInfo
{
    [SerializeField] // ��������GEntity�̖���
    private EnemyName _enemyName;

    [SerializeField, Min(0)] // �����܂ł̎���
    private float _creationDelay;

    [SerializeField] // �����ʒu�̎��
    private SpawnPointSingletonData.SpawnPointType _spawnPointType;

    /// <summary>
    /// ��������GEntity�̖���
    /// </summary>
    public EnemyName MyEnemyName => _enemyName;

    /// <summary>
    /// �����܂ł̎���
    /// </summary>
    public float CreationDelay => _creationDelay;

    /// <summary>
    /// �����ʒu�̎��
    /// </summary>
    public SpawnPointSingletonData.SpawnPointType SpawnPointType => _spawnPointType;
}

/// <summary>
/// �G�����֘A�̏��̔z��̔z��
/// </summary>
[Serializable]
public struct EnemySpawnPatternArray
{
    [Tooltip("�G�����֘A�̏��̔z��̔z��")]
    public EnemySpawnInfo[] arrays;
}

/// <summary>
/// �G�̐����p�^�[���̐ݒ�
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnPatternSettingSO", menuName = "ScriptableObject/EnemySpawnPatternSettingSO")]
public class EnemySpawnPatternSettingSO : ScriptableObject
{
    [SerializeField, Header("�{�X�GPrefab")]
    private Transform _bossEnemyPrefab = null;

    [SerializeField, Header("�G�����֘A�̏��̔z��̔z��̔z��")]
    private EnemySpawnPatternArray[] _patterns = null;

    /// <summary>
    /// �{�X�GPrefab
    /// </summary>
    public Transform BossEnemyPrefab => _bossEnemyPrefab;

    /// <summary>
    /// �G�����֘A�̏��̔z��̔z��̔z��
    /// </summary>
    public EnemySpawnPatternArray[] Patterns => _patterns;
}
