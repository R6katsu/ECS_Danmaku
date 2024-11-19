using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// Entity�ɕϊ�����GPrefab�̐ݒ�
/// </summary>
[Serializable]
public struct EnemyPrefabSetting
{
    [SerializeField, Header("Entity�ɕϊ�����GPrefab")]
    private Transform _enemyPrefabs;

    [SerializeField, Header("�GEntity�̖���")]
    private EnemyName _enemyName;

    /// <summary>
    /// Entity�ɕϊ�����GPrefab
    /// </summary>
    public Transform EnemyPrefabs => _enemyPrefabs;

    /// <summary>
    /// �GEntity�̖���
    /// </summary>
    public EnemyName MyEnemyName => _enemyName;
}

/// <summary>
/// �GEntity�̏���ێ�
/// </summary>
public struct EnemyEntityData : IComponentData
{
    public readonly Entity enemyEntity;
    public readonly EnemyName enemyName;

    /// <summary>
    /// �GEntity�̏���ێ�
    /// </summary>
    /// <param name="enemyEntity">�GEntity</param>
    /// <param name="enemyName">�GEntity�̖���</param>
    public EnemyEntityData(Entity enemyEntity, EnemyName enemyName)
    {
        this.enemyEntity = enemyEntity;
        this.enemyName = enemyName;
    }
}

/// <summary>
/// �GPrefab��Entity�ɕϊ�
/// </summary>
public class EnemyEntityAuthoring : MonoBehaviour
{
    [SerializeField, Header("Entity�ɕϊ�����GPrefab�̐ݒ�")]
    private EnemyPrefabSetting[] _enemyPrefabSettings = null;

    public class Baker : Baker<EnemyEntityAuthoring>
    {
        public override void Bake(EnemyEntityAuthoring src)
        {
            foreach (var enemyPrefabSetting in src._enemyPrefabSettings)
            {
                // ���Entity���쐬
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.None);

                // �GPrefab��Entity�ɕϊ�
                var enemyEntity = GetEntity(enemyPrefabSetting.EnemyPrefabs, TransformUsageFlags.None);

                // �GEntity�̏���ێ�
                var enemyEntityData = new EnemyEntityData
                    (
                        enemyEntity,
                        enemyPrefabSetting.MyEnemyName
                    );

                AddComponent(tmpEnemy, enemyEntityData);
            }
        }
    }
}
