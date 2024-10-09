using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static EnemyHelper;

public class BulletSpawnerAuthoring : MonoBehaviour
{
    [SerializeField, Header("生成する弾のPrefab配列")]
    private GameObject[] _bulletPrefabs = null;

    public GameObject[] BulletPrefabs => _bulletPrefabs;

    public class Baker : Baker<BulletSpawnerAuthoring>
    {
        public override void Bake(BulletSpawnerAuthoring src)
        {
            foreach (var bulletPrefab in src.BulletPrefabs)
            {
                var tmpBullet = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                var bullet = GetEntity(bulletPrefab, TransformUsageFlags.Dynamic);

                var enemySpawnerData = new BulletSpawnerData(bullet);

                AddComponent(tmpBullet, enemySpawnerData);
                AddComponent(tmpBullet, new Disabled());    // 非有効状態に設定
            }
        }
    }
}
