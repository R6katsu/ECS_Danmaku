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
/// n-Way�e�ɕK�v�ȏ��
/// </summary>
public struct N_Way_DanmakuData : IComponentData
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
    public N_Way_DanmakuData(int fanAngle, int amountBullets, float firingInterval, Entity bulletPrefab)
    {
        this.fanAngle = fanAngle;
        this.amountBullets = amountBullets;
        this.firingInterval = firingInterval;
        this.bulletEntity = bulletPrefab;

        // �����l
        elapsedTime = 0.0f;
    }
}

/// <summary>
/// n-Way�e�ɕK�v�Ȑݒ�
/// </summary>
[RequireComponent(typeof(DanmakuTypeSetup))]
public class N_Way_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    private const int MIN_ANGLE = 1;
    private const int MAX_ANGLE = 360;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("��̑傫��")]
    private int _fanAngle = 0;

    [SerializeField, Range(MIN_ANGLE, MAX_ANGLE), Header("�e�̗�")]
    private int _amountBullets = 0;

    [SerializeField, Min(0.0f), Header("���ˊԊu")]
    private float _firingInterval = 0.0f;

    [SerializeField, Header("�e��Prefab")]
    private Transform _bulletPrefab = null;

    public class Baker : Baker<N_Way_DanmakuAuthoring>
    {
        public override void Bake(N_Way_DanmakuAuthoring src)
        {
            var bulletEntity = GetEntity(src._bulletPrefab, TransformUsageFlags.Dynamic);

            var n_Way_DanmakuData = new N_Way_DanmakuData
                (
                    src._fanAngle,
                    src._amountBullets,
                    src._firingInterval,
                    bulletEntity
                );

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), n_Way_DanmakuData);
        }
    }
}
