using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static MoveHelper;

static public class BulletHelper
{
    public struct BulletSpawnerData : IComponentData
    {
        public readonly Entity bulletPrefab;

        public BulletSpawnerData(Entity bulletPrefab)
        {
            this.bulletPrefab = bulletPrefab;
        }
    }
}
