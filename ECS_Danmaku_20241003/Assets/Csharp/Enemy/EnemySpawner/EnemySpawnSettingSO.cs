using System;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;
using static EnemySpawnSettingSO;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// ��������G�̏��
/// </summary>
[Serializable]
public struct EnemySpawnInfo
{
    [SerializeField, Header("�����܂ł̎���")]
    private float _creationDelay;

    [SerializeField, Header("��������G�̖���")]
    private EnemyName _enemyName;

    [SerializeField, Header("��������ʒu")]
    private float3 _spawnPosition;

    /// <summary>
    /// �����܂ł̎���
    /// </summary>
    public float CreationDelay => _creationDelay;

    /// <summary>
    /// ��������G�̖���
    /// </summary>
    public EnemyName EnemyName => _enemyName;
    
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
    private EnemySpawnInfo[] _info = null;

    /// <summary>
    /// ��������G�̏��z��
    /// </summary>
    public EnemySpawnInfo[] Info => _info;

    // �܂��A���߂ɐ����܂ł̕b�����������ɕ��ёւ����z����쐬����
    // �z��̏��Ԃɐ������Ă����BInfo.CreationDelay�b��ҋ@���Ă��玟�̐���������
    // System��Update���Ŏ��̐������Ԃ܂őҋ@���鏈���������B�����łȂ���ΐ؂�グ��

    // �G������System��GameManager���Ő��䂵����
    // �Q�[���J�n�O�ƏI����͓G�𐶐����Ȃ�
    // System���̂�bool����������GameManager���琧�䂷��H
}
