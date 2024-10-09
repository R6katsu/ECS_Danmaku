using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static EnemyHelper;
using static MoveHelper;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SpawneLineCreate : EnemySpawnerAuthoring
{
#if UNITY_EDITOR
    private void OnEnable()
    {
        if (_startPoint == null)
        {
            var startPoint = new GameObject();
            startPoint.name = "startPoint";
            startPoint.transform.parent = transform;
            _startPoint = startPoint.transform;
        }

        if (_endPoint == null)
        {
            var endPoint = new GameObject();
            endPoint.name = "endPoint";
            endPoint.transform.parent = transform;
            _endPoint = endPoint.transform;
        }
    }
#endif

    public class Baker : Baker<SpawneLineCreate>
    {
        public override void Bake(SpawneLineCreate src)
        {
            foreach (var enemyPrefab in src.EnemyPrefabs)
            {
                var tmpEnemy = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                var enemy = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic);

                var enemySpawnerData = new EnemySpawnerData
                    (
                        enemy, 
                        src.MyDirectionTravelType,
                        src.StartPoint.position,
                        src.EndPoint.position
                    );

                AddComponent(tmpEnemy, enemySpawnerData);                
            }
        }
    }
}