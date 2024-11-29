using Unity.Entities;
using UnityEngine;
using static DanmakuHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// �^�b�v�����e���̏��
/// </summary>
public struct TapShooting_DanmakuData : IComponentData, IDataDeletion
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

    public bool IsDataDeletion { get; set; }

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

        currentShotCount = 0;
        singleSetNextTime = 0.0f;
        firingNextTime = 0.0f;

        IsDataDeletion = false;
    }
}

/// <summary>
/// �^�b�v�����e���̐ݒ�
/// </summary>
[RequireComponent(typeof(DanmakuTypeSetup))]
public class TapShooting_DanmakuAuthoring : MonoBehaviour, IDanmakuAuthoring
{
    [SerializeField,Min(0), Header("�����Z�b�g��n�񌂂�")]
    private int _shootNSingleSet = 0;

    [SerializeField, Min(0.0f), Header("�����Z�b�g�I����̋x������")]
    private float _singleSetRestTimeAfter = 0.0f;

    [SerializeField, Min(0.0f), Header("���ˊԊu")]
    private float _firingInterval = 0.0f;

    [SerializeField, Header("�e��Prefab")]
    private Transform _bulletPrefab = null;

    public class Baker : Baker<TapShooting_DanmakuAuthoring>
    {
        public override void Bake(TapShooting_DanmakuAuthoring src)
        {
            var bulletPrefab = GetEntity(src._bulletPrefab, TransformUsageFlags.Dynamic);

            var tapShooting_DanmakuData = new TapShooting_DanmakuData
                (
                    src._shootNSingleSet,
                    src._singleSetRestTimeAfter,
                    src._firingInterval,
                    bulletPrefab
                );
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), tapShooting_DanmakuData);
        }
    }
}
