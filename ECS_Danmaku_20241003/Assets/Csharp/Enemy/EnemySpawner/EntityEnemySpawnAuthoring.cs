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
/// �G�����ݒ�̏��̔z��
/// </summary>
public struct EnemySpawnPattern
{
    [Tooltip("�G�����ݒ�̏��̔z��")]
    public readonly FixedList64Bytes<EnemySpawnInfo> infos;

    /// <summary>
    /// �G�����֘A�̏��̔z��
    /// </summary>
    /// <param name="enemySpawnSettingDatas">�z��̗v�f��FixedList64Bytes�ɑ��</param>
    public EnemySpawnPattern(EnemySpawnInfo[] enemySpawnSettingDatas)
    {
        // �������蓖��
        this.infos = new();

        // �z��̗v�f��FixedList64Bytes�ɑ��
        foreach (var enemySpawnSettingData in enemySpawnSettingDatas)
        {
            this.infos.Add(enemySpawnSettingData);
        }
    }
}

/// <summary>
/// �G�����ݒ�̏��̔z��
/// </summary>
public struct EnemySpawnPatternArraySingletonData : IComponentData
{
    [Tooltip("�G�����ݒ�̏��̔z��")]
    public FixedList4096Bytes<EnemySpawnPattern> infos;

    /// <summary>
    /// �G�����֘A�̏��̔z��
    /// </summary>
    /// <param name="enemySpawnInfoArrayDatas">�z��̗v�f��FixedList4096Bytes�ɑ��</param>
    public EnemySpawnPatternArraySingletonData(EnemySpawnPatternArray[] enemySpawnInfoArrayDatas)
    {
        // �������蓖��
        this.infos = new();
        var tempInfos = new FixedList4096Bytes<EnemySpawnPattern>();

        if (enemySpawnInfoArrayDatas == null)
        {
            Debug.LogError("enemySpawnInfoArrayDatas��null�I");
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

        // �z��̗v�f��FixedList4096Bytes�ɑ��
        this.infos = tempInfos;
    }
}

/// <summary>
/// �����ʒu�̏��i�V���O���g���O��j
/// </summary>
public struct SpawnPointSingletonData : IComponentData
{
    /// <summary>
    /// �����ʒu�̎��
    /// </summary>
    public enum SpawnPointType
    {
        [Tooltip("��")] Left,
        [Tooltip("���S�ƍ��̊�")] LeftSpacer,
        [Tooltip("���S")] Center,
        [Tooltip("���S�ƉE�̊�")] RightSpacer,
        [Tooltip("�E")] Right
    }

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

        // /2���`
        float averageFactor = 2.0f;

        // ���E�ȊO�̐����ʒu���v�Z
        centerPoint = (leftPoint + rightPoint) / averageFactor;
        leftSpacerPoint = (leftPoint + centerPoint) / averageFactor;
        rightSpacerPoint = (rightPoint + centerPoint) / averageFactor;
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
/// �G�̐����ݒ�
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

            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, spawnPointSingletonData);

            // �G�����ݒ�̔z��̔z��
            var aaaa = new EnemySpawnPatternArraySingletonData(src._enemySpawnSettingSO.Patterns);
            AddComponent(entity, aaaa);

            Debug.Log("�������̊m�F�i�K�B���t�@�N�^�����O�K�{");
        }
    }
}