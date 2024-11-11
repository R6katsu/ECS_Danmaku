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
/// �G�����֘A�̏��̐ݒ�
/// </summary>
[Serializable]
public struct EnemySpawnSettingInfo
{
    [SerializeField, Header("�����܂ł̎���")]
    private float _creationDelay;

    [SerializeField, Header("��������ʒu")]
    private float3 _spawnPosition;

    /// <summary>
    /// �����܂ł̎���
    /// </summary>
    public float CreationDelay => _creationDelay;

    /// <summary>
    /// ��������ʒu
    /// </summary>
    public float3 SpawnPosition => _spawnPosition;
}

/// <summary>
/// ��������G�̐ݒ�
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnSettingSO", menuName = "ScriptableObject/EnemySpawnSettingSO")]
public class EnemySpawnSettingSO : ScriptableObject
{
    [SerializeField]
    private TransformEnemySpawnInfo[] _info = null;

    /// <summary>
    /// ��������G�̏��z��
    /// </summary>
    public TransformEnemySpawnInfo[] Info => _info;
}
