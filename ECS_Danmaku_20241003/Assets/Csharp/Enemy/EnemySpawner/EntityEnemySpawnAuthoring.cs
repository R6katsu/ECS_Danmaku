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

// ���t�@�N�^�����O�ς�

/// <summary>
/// �����ʒu�̎��
/// </summary>
public enum SpawnPointType : byte
{
    [Tooltip("��")] Left,
    [Tooltip("���S�ƍ��̊�")] LeftSpacer,
    [Tooltip("���S")] Center,
    [Tooltip("���S�ƉE�̊�")] RightSpacer,
    [Tooltip("�E")] Right
}

/// <summary>
/// �G�����ݒ�̏��̔z��
/// </summary>
public struct EnemySpawnPattern
{
    [Tooltip("�G�����ݒ�̏��̔z��")]
    public readonly FixedList64Bytes<EnemySpawnInfo> enemySpawnInfos;

    /// <summary>
    /// �G�����֘A�̏��̔z��
    /// </summary>
    /// <param name="enemySpawnInfos">�z��̗v�f��FixedList64Bytes�ɑ��</param>
    public EnemySpawnPattern(EnemySpawnInfo[] enemySpawnInfos)
    {
        // �������蓖��
        this.enemySpawnInfos = new();

        // �z��̗v�f��FixedList64Bytes�ɑ��
        foreach (var enemySpawnInfo in enemySpawnInfos)
        {
            this.enemySpawnInfos.Add(enemySpawnInfo);
        }
    }
}

/// <summary>
/// �G�����ݒ�̏��̔z��
/// </summary>
public struct EnemySpawnPatternArraySingletonData : IComponentData
{
    [Tooltip("�G�����ݒ�̏��̔z��")]
    public FixedList4096Bytes<EnemySpawnPattern> enemySpawnPatterns;

    [Tooltip("�{�X�GEntity")]
    public readonly Entity bossEnemyEntity;

    [Tooltip("�{�X�����܂ł̃J�E���g�_�E��")]
    public readonly int countdownBossSpawn;

    /// <summary>
    /// �G�����֘A�̏��̔z��
    /// </summary>
    /// <param name="enemySpawnPatternArrays">�z��̗v�f��FixedList4096Bytes�ɑ��</param>
    /// <param name="bossEnemyEntity">�{�X�GEntity</param>
    /// <param name="countdownBossSpawn">�{�X�����܂ł̃J�E���g�_�E��</param>
    public EnemySpawnPatternArraySingletonData
    (
        EnemySpawnPatternArray[] enemySpawnPatternArrays,
        Entity bossEnemyEntity, 
        int countdownBossSpawn)
    {
        this.bossEnemyEntity = bossEnemyEntity;
        this.countdownBossSpawn = countdownBossSpawn;

        // �������蓖��
        this.enemySpawnPatterns = new();

        if (enemySpawnPatternArrays == null)
        {
#if UNITY_EDITOR
            Debug.LogError("enemySpawnInfoArrayDatas��null");
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
/// �����ʒu�̏��i�V���O���g���O��j
/// </summary>
public struct SpawnPointSingletonData : IComponentData
{
    [Tooltip("���̐����ʒu")]
    public readonly float3 leftPoint;

    [Tooltip("���S�ƍ��̊Ԃ̐����ʒu")]
    public readonly float3 leftSpacerPoint;

    [Tooltip("���S�̐����ʒu")]
    public readonly float3 centerPoint;

    [Tooltip("���S�ƉE�̊Ԃ̐����ʒu")]
    public readonly float3 rightSpacerPoint;

    [Tooltip("�E�̐����ʒu")]
    public readonly float3 rightPoint;

    /// <summary>
    /// �����ʒu
    /// </summary>
    /// <param name="leftPoint">���̐����ʒu</param>
    /// <param name="rightPoint">�E�̐����ʒu</param>
    public SpawnPointSingletonData(float3 leftPoint, float3 rightPoint)
    {
        this.leftPoint = leftPoint;
        this.rightPoint = rightPoint;

        // ���E�ȊO�̐����ʒu���v�Z
        centerPoint = (leftPoint + rightPoint).Halve();
        leftSpacerPoint = (leftPoint + centerPoint).Halve();
        rightSpacerPoint = (rightPoint + centerPoint).Halve();
    }

    /// <summary>
    /// SpawnPointType�ɑΉ����鐶���ʒu��Ԃ�
    /// </summary>
    /// <returns>SpawnPointType�ɑΉ����鐶���ʒu</returns>
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
/// �G�̐����ɕK�v�Ȑݒ�
/// </summary>
public class EntityEnemySpawnAuthoring : SingletonMonoBehaviour<EntityEnemySpawnAuthoring>
{
    [SerializeField]
    private EnemySpawnPatternSettingSO _enemySpawnSettingSO = null;

    [SerializeField, Header("���̐����ʒu")]
    private float3 _leftPoint = float3.zero;

    [SerializeField, Header("�E�̐����ʒu")]
    private float3 _rightPoint = float3.zero;

    public class Baker : Baker<EntityEnemySpawnAuthoring>
    {
        public override void Bake(EntityEnemySpawnAuthoring src)
        {
            // �����ʒu�̏���ێ�����V���O���g�����쐬
            var spawnPointSingletonData = new SpawnPointSingletonData
            (
                src._leftPoint,
                src._rightPoint
            );

            // ����ێ����邾���Ȃ̂�None
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