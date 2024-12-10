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

// ���t�@�N�^�����O�ς�

/// <summary>
/// �G�����ݒ�̏��̐ݒ�
/// </summary>
[Serializable]
public struct EnemySpawnInfo
{
    // ��������GEntity�̖���
    [SerializeField]
    private EnemyName _enemyName;

    // �����܂ł̎���
    [SerializeField, Min(0)]
    private float _creationDelay;

    // �����ʒu�̎��
    [SerializeField]
    private SpawnPointType _spawnPointType;

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
    public SpawnPointType SpawnPointType => _spawnPointType;
}

/// <summary>
/// �G�����֘A�̏��̔z��̔z��
/// </summary>
[Serializable]
public struct EnemySpawnPatternArray
{
    [Tooltip("�G�����֘A�̏��̔z��̔z��")]
    public EnemySpawnInfo[] infos;
}

/// <summary>
/// �G�̐����p�^�[���̐ݒ�
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnPatternSettingSO", menuName = "ScriptableObject/EnemySpawnPatternSettingSO")]
public class EnemySpawnPatternSettingSO : ScriptableObject
{
    [SerializeField, Header("�{�X�GPrefab")]
    private Transform _bossEnemyPrefab = null;

    [SerializeField, Header("�{�X�����܂ł̃J�E���g�_�E��")]
    private int _countdownBossSpawn;

    [SerializeField, Header("�G�����֘A�̏��̔z��̔z��̔z��")]
    private EnemySpawnPatternArray[] _patternArrays = null;

    /// <summary>
    /// �{�X�GPrefab
    /// </summary>
    public Transform BossEnemyPrefab => _bossEnemyPrefab;

    /// <summary>
    /// �{�X�����܂ł̃J�E���g�_�E��
    /// </summary>
    public int CountdownBossSpawn => _countdownBossSpawn;

    /// <summary>
    /// �G�����֘A�̏��̔z��̔z��̔z��
    /// </summary>
    public EnemySpawnPatternArray[] PatternArrays => _patternArrays;
}
