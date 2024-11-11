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
/// 敵Transform + 敵生成関連の情報の設定
/// </summary>
[Serializable]
public struct TransformEnemySpawnInfo
{
    [SerializeField, Header("生成する敵Prefab")]
    private Transform _enemyPrefab;

    [SerializeField, Header("敵生成関連の情報")]
    private EnemySpawnSettingInfo _enemySpawnSettingInfo;

    /// <summary>
    /// 生成する敵Prefab
    /// </summary>
    public Transform EnemyPrefab => _enemyPrefab;

    /// <summary>
    /// 敵生成関連の情報
    /// </summary>
    public EnemySpawnSettingInfo EnemySpawnSettingInfo => _enemySpawnSettingInfo;
}

/// <summary>
/// 敵Entity + 敵生成関連の情報の設定
/// </summary>
public struct EntityEnemySpawnData : IComponentData
{
    [Tooltip("生成する敵Entity")]
    public readonly Entity enemyEntity;

    [Tooltip("敵生成関連の情報")]
    public readonly EnemySpawnSettingInfo enemySpawnSettingInfo;

    /// <summary>
    /// 敵Entity + 敵生成関連の情報の設定
    /// </summary>
    /// <param name="enemySpawnSettingInfo">敵生成関連の情報</param>
    /// <param name="enemyEntity">生成する敵Entity</param>
    public EntityEnemySpawnData(Entity enemyEntity, EnemySpawnSettingInfo enemySpawnSettingInfo)
    {
        this.enemyEntity = enemyEntity;
        this.enemySpawnSettingInfo = enemySpawnSettingInfo;
    }
}

/// <summary>
/// 敵の生成設定
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
                // 敵PrefabをEntityに変換し、Dataのコンストラクタに渡す
                var enemyEntity = GetEntity(info.EnemyPrefab, TransformUsageFlags.Dynamic);
                var entityEnemySpawnInfo = new EntityEnemySpawnData(enemyEntity, info.EnemySpawnSettingInfo);

                // 空のEntityを作成
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.None);

                // 空のEntityにEntityEnemySpawnInfoをアタッチ
                AddComponent(tmpEnemy, entityEnemySpawnInfo);
            }
        }
    }
}