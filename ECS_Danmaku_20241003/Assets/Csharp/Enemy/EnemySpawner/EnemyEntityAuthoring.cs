using System;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// Entity‚É•ÏŠ·‚·‚é“GPrefab‚Ìİ’è
/// </summary>
[Serializable]
public struct EnemyPrefabSetting
{
    [SerializeField, Header("Entity‚É•ÏŠ·‚·‚é“GPrefab")]
    private Transform _enemyPrefabs;

    [SerializeField, Header("“GEntity‚Ì–¼Ì")]
    private EnemyName _enemyName;

    /// <summary>
    /// Entity‚É•ÏŠ·‚·‚é“GPrefab
    /// </summary>
    public Transform EnemyPrefabs => _enemyPrefabs;

    /// <summary>
    /// “GEntity‚Ì–¼Ì
    /// </summary>
    public EnemyName MyEnemyName => _enemyName;
}

/// <summary>
/// “GEntity‚Ìî•ñ‚ğ•Û
/// </summary>
public struct EnemyEntityData : IComponentData
{
    public readonly Entity enemyEntity;
    public readonly EnemyName enemyName;

    /// <summary>
    /// “GEntity‚Ìî•ñ‚ğ•Û
    /// </summary>
    /// <param name="enemyEntity">“GEntity</param>
    /// <param name="enemyName">“GEntity‚Ì–¼Ì</param>
    public EnemyEntityData(Entity enemyEntity, EnemyName enemyName)
    {
        this.enemyEntity = enemyEntity;
        this.enemyName = enemyName;
    }
}

/// <summary>
/// “GPrefab‚ğEntity‚É•ÏŠ·
/// </summary>
public class EnemyEntityAuthoring : MonoBehaviour
{
    [SerializeField, Header("Entity‚É•ÏŠ·‚·‚é“GPrefab‚Ìİ’è")]
    private EnemyPrefabSetting[] _enemyPrefabSettings = null;

    public class Baker : Baker<EnemyEntityAuthoring>
    {
        public override void Bake(EnemyEntityAuthoring src)
        {
            foreach (var enemyPrefabSetting in src._enemyPrefabSettings)
            {
                // ‹ó‚ÌEntity‚ğì¬
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.None);

                // “GPrefab‚ğEntity‚É•ÏŠ·
                var enemyEntity = GetEntity(enemyPrefabSetting.EnemyPrefabs, TransformUsageFlags.None);

                // “GEntity‚Ìî•ñ‚ğ•Û
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
