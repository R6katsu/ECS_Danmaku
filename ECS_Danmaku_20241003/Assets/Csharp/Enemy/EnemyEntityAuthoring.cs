using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �GEntityList�̏��
/// </summary>
public struct EnemyEntitySingletonData :IComponentData
{
    public readonly FixedList4096Bytes<Entity> enemyEntitys;

    /// <summary>
    /// �GEntityList�̏��
    /// </summary>
    /// <param name="enemyEntitys">�GEntityList</param>
    public EnemyEntitySingletonData(FixedList4096Bytes<Entity> enemyEntitys)
    {
        this.enemyEntitys = enemyEntitys;
    }

    /// <summary>
    /// EnemyName�ɑΉ�����GEntity��Ԃ�
    /// </summary>
    /// <param name="name">�GEntity�̖���</param>
    /// <returns>EnemyName�ɑΉ�����GEntity</returns>
    public Entity GetEnemyEntity(EnemyName name)
    {
        return enemyEntitys[(byte)name];
    }
}

/// <summary>
/// �G��Prefab��Entity������ׂ̐ݒ�
/// </summary>
public class EnemyEntityAuthoring : SingletonMonoBehaviour<EnemyEntityAuthoring>
{
    [SerializeField, Header("�GPrefab�̔z��")]
    private Transform[] _enemyPrefabs = null;

    public class Baker : Baker<EnemyEntityAuthoring>
    {
        public override void Bake(EnemyEntityAuthoring src)
        {
            var enemyPrefabs = src._enemyPrefabs;

            // List��������
            var entityList = new FixedList4096Bytes<Entity>();

            foreach (var enemyPrefab in enemyPrefabs)
            {
                // �GPrefab��Entity�ɕϊ�
                var enemyEntity = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                // List�ɓGEntity��ǉ�
                entityList.Add(enemyEntity);
            }

            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var enemyEntitySingleton = new EnemyEntitySingletonData
                (
                    entityList
                );

            AddComponent(entity, enemyEntitySingleton);

            // �r���h���ɌŒ�
            AddComponent<StaticOptimizeEntity>(entity);
        }
    }
}
