using System;
using Unity.Entities;
using UnityEngine;



#if UNITY_EDITOR
using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using static MoveHelper;
using System.Security.Cryptography;
using Unity.Entities.UniversalDelegates;
using System.Reflection;
using System.Linq;
using Unity.Mathematics;
using System.Net;
using static EnemyHelper;
using static UnityEngine.EventSystems.EventTrigger;
#endif

/// <summary>
/// �GTransform + �G�����֘A�̏��̐ݒ�
/// </summary>
[Serializable]
public struct TransformEnemySpawnInfo
{
    [SerializeField, Header("��������GPrefab")]
    private Transform _enemyPrefab;

    [SerializeField, Header("�G�����֘A�̏��")]
    private EnemySpawnSettingInfo _enemySpawnSettingInfo;

    /// <summary>
    /// ��������GPrefab
    /// </summary>
    public Transform EnemyPrefab => _enemyPrefab;

    /// <summary>
    /// �G�����֘A�̏��
    /// </summary>
    public EnemySpawnSettingInfo EnemySpawnSettingInfo => _enemySpawnSettingInfo;
}

/// <summary>
/// �GEntity + �G�����֘A�̏��̐ݒ�
/// </summary>
public struct EntityEnemySpawnData : IComponentData
{
    [Tooltip("��������GEntity")]
    public readonly Entity enemyEntity;

    [Tooltip("�G�����֘A�̏��")]
    public readonly EnemySpawnSettingInfo enemySpawnSettingInfo;

    /// <summary>
    /// �GEntity + �G�����֘A�̏��̐ݒ�
    /// </summary>
    /// <param name="enemySpawnSettingInfo">�G�����֘A�̏��</param>
    /// <param name="enemyEntity">��������GEntity</param>
    public EntityEnemySpawnData(Entity enemyEntity, EnemySpawnSettingInfo enemySpawnSettingInfo)
    {
        this.enemyEntity = enemyEntity;
        this.enemySpawnSettingInfo = enemySpawnSettingInfo;
    }
}

/// <summary>
/// �G�̐����ݒ�
/// </summary>
public class EntityEnemySpawnAuthoring : SingletonMonoBehaviour<EntityEnemySpawnAuthoring>
{
    [SerializeField]
    private EnemySpawnSettingSO _enemySpawnSettingSO = null;

    public class Baker : Baker<EntityEnemySpawnAuthoring>
    {
        public override void Bake(EntityEnemySpawnAuthoring src)
        {
            foreach (var info in src._enemySpawnSettingSO.Info)
            {
                // �GPrefab��Entity�ɕϊ����AData�̃R���X�g���N�^�ɓn��
                var enemyEntity = GetEntity(info.EnemyPrefab, TransformUsageFlags.Dynamic);
                var entityEnemySpawnInfo = new EntityEnemySpawnData(enemyEntity, info.EnemySpawnSettingInfo);

                // ���Entity���쐬
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.None);

                // ���Entity��EntityEnemySpawnInfo���A�^�b�`
                AddComponent(tmpEnemy, entityEnemySpawnInfo);
            }
        }
    }
}