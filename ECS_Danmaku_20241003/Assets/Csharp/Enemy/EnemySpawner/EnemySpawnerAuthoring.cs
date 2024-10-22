using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;
using static EnemyHelper;

#if UNITY_EDITOR
using System.Reflection;
using System.Linq;
using Unity.Mathematics;
using System.Net;
#endif

/// <summary>
/// �G�̐����ݒ�
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class EnemySpawnerAuthoring : MonoBehaviour
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

    /// <summary>
    /// ���������G�̈ړ�����
    /// </summary>
    public DirectionTravelType MyDirectionTravelType => _directionTravelType;

    /// <summary>
    /// ��������ʒu�̐��̎n�_
    /// </summary>
    public Transform StartPoint => _startPoint;

    /// <summary>
    /// ��������ʒu�̐��̏I�_
    /// </summary>
    public Transform EndPoint => _endPoint;

    /// <summary>
    /// ��������G��Prefab�z��
    /// </summary>
    public GameObject[] EnemyPrefabs => _enemyPrefabs;

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
            foreach (var enemyPrefab in src.EnemyPrefabs)
            {
                var enemy = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                // �G�𐶐�����ׂɕK�v�ȏ����쐬
                var enemySpawnerData = new EnemySpawnerData
                    (
                        enemy,
                        src.MyDirectionTravelType,
                        src.StartPoint.position,
                        src.EndPoint.position
                    );

                // ���Entity���쐬���A�G�̐����ɕK�v�ȏ����A�^�b�`����
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                AddComponent(tmpEnemy, enemySpawnerData);
            }
        }
    }
}