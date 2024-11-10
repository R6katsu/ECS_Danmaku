using System;
using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

#if UNITY_EDITOR
using System.Reflection;
using System.Linq;
using Unity.Mathematics;
using System.Net;
using static EnemyHelper;
#endif

/// <summary>
/// �G�̐����ɕK�v�ȏ��i�V���O���g���O��j
/// </summary>
public struct EnemySpawnerSingletonData : IComponentData
{
    [Tooltip("��������Prefab��Entity")]
    public readonly Entity enemyEntity;

    [Tooltip("������̈ړ�����")]
    public readonly DirectionTravelType directionTravelType;

    [Tooltip("����������̎n�_")]
    public readonly Vector3 startPoint;

    [Tooltip("����������̏I�_")]
    public readonly Vector3 endPoint;

    public readonly FixedList4096Bytes<EnemySpawnInfo> enemySpawnInfos;

    /// <summary>
    /// �G�̐����ɕK�v�ȏ��
    /// </summary>
    /// <param name="enemyEntity">��������Prefab��Entity</param>
    /// <param name="directionTravelType">������̈ړ�����</param>
    /// <param name="startPoint">����������̎n�_</param>
    /// <param name="endPoint">����������̏I�_</param>
    /// <param name="enemySpawnInfos">�G��������List</param>
    public EnemySpawnerSingletonData(
        Entity enemyEntity,
        DirectionTravelType directionTravelType,
        Vector3 startPoint,
        Vector3 endPoint,
        FixedList4096Bytes<EnemySpawnInfo> enemySpawnInfos)
    {
        this.enemyEntity = enemyEntity;
        this.directionTravelType = directionTravelType;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.enemySpawnInfos = enemySpawnInfos;
    }
}

/// <summary>
/// �G�̐����ݒ�
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class EnemySpawnerAuthoring : SingletonMonoBehaviour<EnemySpawnerAuthoring>
{
    // ����������̉���
    void OnDrawGizmos()
    {
        if (_startPoint != null && _endPoint != null)
        {
            // ���̐F��ݒ�
            Gizmos.color = Color.red;

            // �n�_�ƏI�_�����Ԑ���`��
            Gizmos.DrawLine(_startPoint.position, _endPoint.position);
        }
    }

    [SerializeField, Header("���������G�̈ړ�����")]
    private DirectionTravelType _directionTravelType = 0;

    [Header("��������ʒu�̐��������ׂ̎n�_�ƏI�_��ݒ肷��")]
    [SerializeField] protected Transform _startPoint = null;
    [SerializeField] protected Transform _endPoint = null;

    [SerializeField, Header("��������G��Prefab�z��")]
    private GameObject[] _enemyPrefabs = null;

    [SerializeField]
    private EnemySpawnSettingSO _enemySpawnSettingSO = null;

#if UNITY_EDITOR
    private void OnEnable()
    {
        // �n�_��null��������쐬����
        if (_startPoint == null)
        {
            var startPoint = new GameObject();
            startPoint.name = "startPoint";
            startPoint.transform.parent = transform;
            _startPoint = startPoint.transform;
        }

        // �I�_��null��������쐬����
        if (_endPoint == null)
        {
            var endPoint = new GameObject();
            endPoint.name = "endPoint";
            endPoint.transform.parent = transform;
            _endPoint = endPoint.transform;
        }
    }
#endif

    public class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring src)
        {
            foreach (var enemyPrefab in src._enemyPrefabs)
            {
                var enemy = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                var enemySpawnInfos = new FixedList4096Bytes<EnemySpawnInfo>();
                foreach (var info in src._enemySpawnSettingSO.Info)
                {
                    enemySpawnInfos.Add(info);
                }

                // �G�𐶐�����ׂɕK�v�ȏ����쐬
                var enemySpawnerData = new EnemySpawnerSingletonData
                    (
                        enemy,
                        src._directionTravelType,
                        src._startPoint.position,
                        src._endPoint.position,
                        enemySpawnInfos
                    );

                // ���Entity���쐬���A�G�̐����ɕK�v�ȏ����A�^�b�`����
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                AddComponent(tmpEnemy, enemySpawnerData);
            }
        }
    }
}