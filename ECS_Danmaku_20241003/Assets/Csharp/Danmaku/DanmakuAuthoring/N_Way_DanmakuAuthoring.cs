using System;
using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// Way�e�ɕK�v�ȃp�����[�^�̐ݒ�
/// </summary>
[Serializable]
public struct NWay_DanmakuSettingParameter
{
    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("��̑傫��")]
    private int _fanAngle;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("�e�̗�")]
    private int _amountBullets;

    [SerializeField, Header("���ˊԊu")]
    private float _firingInterval;

    [SerializeField, Header("�e��Entity")]
    private Transform _bulletPrefab;

    [Tooltip("�o�ߎ���")]
    private float _elapsedTime;

    /// <summary>
    /// ��̑傫��
    /// </summary>
    public int FanAngle =>_fanAngle;

    /// <summary>
    /// �e�̗�
    /// </summary>
    public int AmountBullets => _amountBullets;

    /// <summary>
    /// ���ˊԊu
    /// </summary>
    public float FiringInterval => _firingInterval;

    /// <summary>
    /// �e��Entity
    /// </summary>
    public Transform BulletPrefab => _bulletPrefab;

    /// <summary>
    /// �o�ߎ���
    /// </summary>
    public float ElapsedTime =>_elapsedTime;
}

/// <summary>
/// n-Way�e�ɕK�v�ȏ��
/// </summary>
public struct NWay_DanmakuData : IComponentData
{
    [Tooltip("��̑傫��")]
    public readonly int fanAngle;

    [Tooltip("�e�̗�")]
    public readonly int amountBullets;

    [Tooltip("���ˊԊu")]
    public readonly float firingInterval;

    [Tooltip("�e��Entity")]
    public readonly Entity bulletEntity;

    [Tooltip("�o�ߎ���")]
    public float elapsedTime;

    /// <summary>
    /// n-Way�e
    /// </summary>
    /// <param name="fanAngle">��̑傫��</param>
    /// <param name="amountBullets">�e�̗�</param>
    /// <param name="firingInterval">���ˊԊu</param>
    /// <param name="bulletPrefab">�e��Prefab</param>
    public NWay_DanmakuData(int fanAngle, int amountBullets, float firingInterval, Entity bulletPrefab)
    {
        this.fanAngle = fanAngle;
        this.amountBullets = amountBullets;
        this.firingInterval = firingInterval;
        this.bulletEntity = bulletPrefab;

        // �����l
        elapsedTime = 0.0f;
    }

    /// <summary>
    /// n-Way�e
    /// </summary>
    /// <param name="nWay_DanmakuSettingParam">Way�e�ɕK�v�ȃp�����[�^�̐ݒ�</param>
    /// <param name="bulletPrefab">�e��Prefab</param>
    public NWay_DanmakuData(NWay_DanmakuSettingParameter nWay_DanmakuSettingParam, Entity bulletPrefab)
    {
        this.fanAngle = nWay_DanmakuSettingParam.FanAngle;
        this.amountBullets = nWay_DanmakuSettingParam.AmountBullets;
        this.firingInterval = nWay_DanmakuSettingParam.FiringInterval;
        this.bulletEntity = bulletPrefab;

        // �����l
        elapsedTime = 0.0f;
    }
}

/// <summary>
/// n-Way�e�ɕK�v�Ȑݒ�
/// </summary>
#if UNITY_EDITOR
[RequireComponent(typeof(DanmakuTypeSetup))]
#endif
public class N_Way_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField, Header("Way�e�ɕK�v�ȃp�����[�^�̐ݒ�")]
    private NWay_DanmakuSettingParameter _nWay_DanmakuSettingParameter = new();

    public class Baker : Baker<N_Way_DanmakuAuthoring>
    {
        public override void Bake(N_Way_DanmakuAuthoring src)
        {
            var bulletEntity = GetEntity(src._nWay_DanmakuSettingParameter.BulletPrefab, TransformUsageFlags.Dynamic);

            var n_Way_DanmakuData = new NWay_DanmakuData
            (
                src._nWay_DanmakuSettingParameter,
                bulletEntity
            );

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), n_Way_DanmakuData);
        }
    }
}
