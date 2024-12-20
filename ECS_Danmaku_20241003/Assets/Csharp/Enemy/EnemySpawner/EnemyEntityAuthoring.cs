using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// Entityに変換する敵Prefabの設定
/// </summary>
[Serializable]
public struct EnemyPrefabSetting
{
    [SerializeField, Header("Entityに変換する敵Prefab")]
    private Transform _enemyPrefabs;

    [SerializeField, Header("敵Entityの名称")]
    private EnemyType _enemyName;

    /// <summary>
    /// Entityに変換する敵Prefab
    /// </summary>
    public Transform EnemyPrefabs => _enemyPrefabs;

    /// <summary>
    /// 敵Entityの名称
    /// </summary>
    public EnemyType MyEnemyName => _enemyName;
}

/// <summary>
/// 敵Entityの情報を保持
/// </summary>
public struct EnemyEntityData : IComponentData
{
    public readonly Entity enemyEntity;
    public readonly EnemyType enemyName;

    /// <summary>
    /// 敵Entityの情報を保持
    /// </summary>
    /// <param name="enemyEntity">敵Entity</param>
    /// <param name="enemyName">敵Entityの名称</param>
    public EnemyEntityData(Entity enemyEntity, EnemyType enemyName)
    {
        this.enemyEntity = enemyEntity;
        this.enemyName = enemyName;
    }
}

/// <summary>
/// 敵PrefabをEntityに変換
/// </summary>
public class EnemyEntityAuthoring : MonoBehaviour
{
    [SerializeField, Header("Entityに変換する敵Prefabの設定")]
    private EnemyPrefabSetting[] _enemyPrefabSettings = null;

    public class Baker : Baker<EnemyEntityAuthoring>
    {
        public override void Bake(EnemyEntityAuthoring src)
        {
            foreach (var enemyPrefabSetting in src._enemyPrefabSettings)
            {
                // 空のEntityを作成、情報を保持するだけなのでNone
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.None);

                // 敵PrefabをEntityに変換
                // 複製の為に参照するだけなのでNone
                var enemyEntity = GetEntity(enemyPrefabSetting.EnemyPrefabs, TransformUsageFlags.None);

                // 敵Entityの情報を保持
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
