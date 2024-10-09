using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Entities;
using System.Reflection;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    // SpawneLineCreate��Bake�����Ă���ׁA�����炪�@�\���Ȃ�
    // ����Script���A�^�b�`������SpawneLineCreate���A�^�b�`����̂��Ȃ񂩈Ⴄ
    // ����������ɕ����Ȃ����@���l��������������������Ȃ�


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

    [SerializeField, Header("��������ʒu�̐��������ׂ̎n�_�ƏI�_��ݒ肷��")]
    protected Transform _startPoint = null;
    [SerializeField]
    protected Transform _endPoint = null;

    [SerializeField, Header("��������G��Prefab�z��")]
    private GameObject[] _enemyPrefabs = null;

    public DirectionTravelType MyDirectionTravelType => _directionTravelType;
    public Transform StartPoint => _startPoint;
    public Transform EndPoint => _endPoint;
    public GameObject[] EnemyPrefabs => _enemyPrefabs;
}