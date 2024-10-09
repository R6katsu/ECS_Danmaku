using Unity.Entities;
using UnityEngine;
using static MoveHelper;

static public class EnemyHelper
{
    public struct EnemyTag : IComponentData{ }

    public struct EnemySpawnerData : IComponentData
    {
        public readonly Entity enemyPrefab;
        public readonly DirectionTravelType directionTravelType;
        public readonly Vector3 startPoint;
        public readonly Vector3 endPoint;

        public EnemySpawnerData(
            Entity enemyPrefab,
            DirectionTravelType directionTravelType,
            Vector3 startPoint, 
            Vector3 endPoint)
        {
            this.enemyPrefab = enemyPrefab;
            this.directionTravelType = directionTravelType;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
    }
}
