using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// “GEntityList‚Ìî•ñ
/// </summary>
public struct EnemyEntitySingletonData :IComponentData
{
    public readonly FixedList4096Bytes<Entity> enemyEntitys;

    /// <summary>
    /// “GEntityList‚Ìî•ñ
    /// </summary>
    /// <param name="enemyEntitys">“GEntityList</param>
    public EnemyEntitySingletonData(FixedList4096Bytes<Entity> enemyEntitys)
    {
        this.enemyEntitys = enemyEntitys;
    }

    /// <summary>
    /// EnemyName‚É‘Î‰‚·‚é“GEntity‚ğ•Ô‚·
    /// </summary>
    /// <param name="name">“GEntity‚Ì–¼Ì</param>
    /// <returns>EnemyName‚É‘Î‰‚·‚é“GEntity</returns>
    public Entity GetEnemyEntity(EnemyName name)
    {
        return enemyEntitys[(byte)name];
    }
}

/// <summary>
/// “G‚ÌPrefab‚ğEntity‰»‚·‚éˆ×‚Ìİ’è
/// </summary>
public class EnemyEntityAuthoring : SingletonMonoBehaviour<EnemyEntityAuthoring>
{
    [SerializeField, Header("“GPrefab‚Ì”z—ñ")]
    private Transform[] _enemyPrefabs = null;

    public class Baker : Baker<EnemyEntityAuthoring>
    {
        public override void Bake(EnemyEntityAuthoring src)
        {
            var enemyPrefabs = src._enemyPrefabs;

            // List‚ğ‰Šú‰»
            var entityList = new FixedList4096Bytes<Entity>();

            foreach (var enemyPrefab in enemyPrefabs)
            {
                // “GPrefab‚ğEntity‚É•ÏŠ·
                var enemyEntity = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                // List‚É“GEntity‚ğ’Ç‰Á
                entityList.Add(enemyEntity);
            }

            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var enemyEntitySingleton = new EnemyEntitySingletonData
                (
                    entityList
                );

            AddComponent(entity, enemyEntitySingleton);

            // ƒrƒ‹ƒh‚ÉŒÅ’è
            AddComponent<StaticOptimizeEntity>(entity);
        }
    }
}
