using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// 敵EntityListの情報
/// </summary>
public struct EnemyEntitySingletonData :IComponentData
{
    public readonly FixedList4096Bytes<Entity> enemyEntitys;

    /// <summary>
    /// 敵EntityListの情報
    /// </summary>
    /// <param name="enemyEntitys">敵EntityList</param>
    public EnemyEntitySingletonData(FixedList4096Bytes<Entity> enemyEntitys)
    {
        this.enemyEntitys = enemyEntitys;
    }

    /// <summary>
    /// EnemyNameに対応する敵Entityを返す
    /// </summary>
    /// <param name="name">敵Entityの名称</param>
    /// <returns>EnemyNameに対応する敵Entity</returns>
    public Entity GetEnemyEntity(EnemyName name)
    {
        return enemyEntitys[(byte)name];
    }
}

/// <summary>
/// 敵のPrefabをEntity化する為の設定
/// </summary>
public class EnemyEntityAuthoring : SingletonMonoBehaviour<EnemyEntityAuthoring>
{
    [SerializeField, Header("敵Prefabの配列")]
    private Transform[] _enemyPrefabs = null;

    public class Baker : Baker<EnemyEntityAuthoring>
    {
        public override void Bake(EnemyEntityAuthoring src)
        {
            var enemyPrefabs = src._enemyPrefabs;

            // Listを初期化
            var entityList = new FixedList4096Bytes<Entity>();

            foreach (var enemyPrefab in enemyPrefabs)
            {
                // 敵PrefabをEntityに変換
                var enemyEntity = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                // Listに敵Entityを追加
                entityList.Add(enemyEntity);
            }

            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var enemyEntitySingleton = new EnemyEntitySingletonData
                (
                    entityList
                );

            AddComponent(entity, enemyEntitySingleton);

            // ビルド時に固定
            AddComponent<StaticOptimizeEntity>(entity);
        }
    }
}
