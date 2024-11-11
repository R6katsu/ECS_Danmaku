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
/// “GTransform + “G¶¬ŠÖ˜A‚Ìî•ñ‚Ìİ’è
/// </summary>
[Serializable]
public struct TransformEnemySpawnInfo
{
    [SerializeField, Header("¶¬‚·‚é“GPrefab")]
    private Transform _enemyPrefab;

    [SerializeField, Header("“G¶¬ŠÖ˜A‚Ìî•ñ")]
    private EnemySpawnSettingInfo _enemySpawnSettingInfo;

    /// <summary>
    /// ¶¬‚·‚é“GPrefab
    /// </summary>
    public Transform EnemyPrefab => _enemyPrefab;

    /// <summary>
    /// “G¶¬ŠÖ˜A‚Ìî•ñ
    /// </summary>
    public EnemySpawnSettingInfo EnemySpawnSettingInfo => _enemySpawnSettingInfo;
}

/// <summary>
/// “GEntity + “G¶¬ŠÖ˜A‚Ìî•ñ‚Ìİ’è
/// </summary>
public struct EntityEnemySpawnData : IComponentData
{
    [Tooltip("¶¬‚·‚é“GEntity")]
    public readonly Entity enemyEntity;

    [Tooltip("“G¶¬ŠÖ˜A‚Ìî•ñ")]
    public readonly EnemySpawnSettingInfo enemySpawnSettingInfo;

    /// <summary>
    /// “GEntity + “G¶¬ŠÖ˜A‚Ìî•ñ‚Ìİ’è
    /// </summary>
    /// <param name="enemySpawnSettingInfo">“G¶¬ŠÖ˜A‚Ìî•ñ</param>
    /// <param name="enemyEntity">¶¬‚·‚é“GEntity</param>
    public EntityEnemySpawnData(Entity enemyEntity, EnemySpawnSettingInfo enemySpawnSettingInfo)
    {
        this.enemyEntity = enemyEntity;
        this.enemySpawnSettingInfo = enemySpawnSettingInfo;
    }
}

/// <summary>
/// “G‚Ì¶¬İ’è
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
                // “GPrefab‚ğEntity‚É•ÏŠ·‚µAData‚ÌƒRƒ“ƒXƒgƒ‰ƒNƒ^‚É“n‚·
                var enemyEntity = GetEntity(info.EnemyPrefab, TransformUsageFlags.Dynamic);
                var entityEnemySpawnInfo = new EntityEnemySpawnData(enemyEntity, info.EnemySpawnSettingInfo);

                // ‹ó‚ÌEntity‚ğì¬
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.None);

                // ‹ó‚ÌEntity‚ÉEntityEnemySpawnInfo‚ğƒAƒ^ƒbƒ`
                AddComponent(tmpEnemy, entityEnemySpawnInfo);
            }
        }
    }
}