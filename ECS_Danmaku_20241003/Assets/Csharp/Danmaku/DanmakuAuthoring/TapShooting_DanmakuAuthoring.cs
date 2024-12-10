using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;
using System;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �^�b�v�����e�ɕK�v�ȃp�����[�^�̐ݒ�
/// </summary>
[Serializable]
public struct TapShootingSettingParameter
{
    [SerializeField, Header("�����Z�b�g��n�񌂂�")]
    private int _shootNSingleSet;

    [SerializeField, Header("�����Z�b�g�I����̋x������")]
    private float _singleSetRestTimeAfter;

    [SerializeField, Header("���ˊԊu")]
    private float _firingInterval;

    [SerializeField, Header("�e��Prefab")]
    private Transform _bulletPrefab;

    /// <summary>
    /// �����Z�b�g��n�񌂂�
    /// </summary>
    public int ShootNSingleSet => _shootNSingleSet;

    /// <summary>
    /// �����Z�b�g�I����̋x������
    /// </summary>
    public float SingleSetRestTimeAfter => _singleSetRestTimeAfter;

    /// <summary>
    /// ���ˊԊu
    /// </summary>
    public float FiringInterval => _firingInterval;

    /// <summary>
    /// �e��Prefab
    /// </summary>
    public Transform BulletPrefab => _bulletPrefab;
}

/// <summary>
/// �^�b�v�����e���ɕK�v�ȏ��
/// </summary>
public struct TapShooting_DanmakuData : IComponentData
{
    [Tooltip("�����Z�b�g��n�񌂂�")]
    public readonly int shootNSingleSet;

    [Tooltip("�����Z�b�g�I����̋x������")]
    public readonly float singleSetRestTimeAfter;

    [Tooltip("���ˊԊu")]
    public readonly float firingInterval;

    [Tooltip("�e��PrefabEntity")]
    public readonly Entity bulletEntity;

    [Tooltip("���݂̎ˌ���")]
    public int currentShotCount;

    [Tooltip("����̃����Z�b�g�J�n����")]
    public double singleSetNextTime;

    [Tooltip("����̎ˌ�����")]
    public double firingNextTime;

    /// <summary>
    /// �^�b�v�����e���̏��
    /// </summary>
    /// <param name="shootNSingleSet">�����Z�b�g��n�񌂂�</param>
    /// <param name="singleSetRestTimeAfter">�����Z�b�g�I����̋x������</param>
    /// <param name="firingInterval">���ˊԊu</param>
    /// <param name="bulletEntity">�e��PrefabEntity</param>
    public TapShooting_DanmakuData(int shootNSingleSet, float singleSetRestTimeAfter, float firingInterval, Entity bulletEntity)
    {
        this.shootNSingleSet = shootNSingleSet;
        this.singleSetRestTimeAfter = singleSetRestTimeAfter;
        this.firingInterval = firingInterval;
        this.bulletEntity = bulletEntity;

        // �����l
        currentShotCount = 0;
        singleSetNextTime = 0.0f;
        firingNextTime = 0.0f;
    }

    /// <summary>
    /// �^�b�v�����e���̏��
    /// </summary>
    /// <param name="tapShootingSettingParameter">�^�b�v�����e�ɕK�v�ȃp�����[�^�̐ݒ�</param>
    /// <param name="bulletEntity">�e��PrefabEntity</param>
    public TapShooting_DanmakuData(TapShootingSettingParameter tapShootingSettingParameter, Entity bulletEntity)
    {
        this.shootNSingleSet = tapShootingSettingParameter.ShootNSingleSet;
        this.singleSetRestTimeAfter = tapShootingSettingParameter.SingleSetRestTimeAfter;
        this.firingInterval = tapShootingSettingParameter.FiringInterval;
        this.bulletEntity = bulletEntity;

        // �����l
        currentShotCount = 0;
        singleSetNextTime = 0.0f;
        firingNextTime = 0.0f;
    }
}

/// <summary>
/// �^�b�v�����e���ɕK�v�Ȑݒ�
/// </summary>
#if UNITY_EDITOR
[RequireComponent(typeof(DanmakuTypeSetup))]
#endif
public class TapShooting_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField, Header("�^�b�v�����e�ɕK�v�ȃp�����[�^�̐ݒ�")]
    private TapShootingSettingParameter _tapShootingSettingParameter = new();

    public class Baker : Baker<TapShooting_DanmakuAuthoring>
    {
        public override void Bake(TapShooting_DanmakuAuthoring src)
        {
            var bulletPrefab = GetEntity(src._tapShootingSettingParameter.BulletPrefab, TransformUsageFlags.Dynamic);

            var tapShooting_DanmakuData = new TapShooting_DanmakuData
            (
                src._tapShootingSettingParameter,
                bulletPrefab
            );

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), tapShooting_DanmakuData);
        }
    }
}
