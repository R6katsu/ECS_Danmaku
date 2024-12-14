using Unity.Entities;
using UnityEngine;
using static HealthPointDatas;

#if UNITY_EDITOR
using static EntityCategoryHelper;
using System.Collections;
using System.Collections.Generic;
using static EntityCampsHelper;
#endif

// リファクタリング済み

/// <summary>
/// 敵の設定
/// </summary>
public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField, Min(0.0f), Header("最大体力")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0), Header("被弾時の効果音番号")]
    private int _hitSENumber = 0;

    [SerializeField, Min(0), Header("死亡時の効果音番号")]
    private int _killedSENumber = 0;

    [SerializeField, Header("陣営の種類")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entityのカテゴリ")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var healthPointData = new HealthPointData
            (
                src._maxHP,
                src._hitSENumber,
                src._killedSENumber
            );

            // Dataをアタッチ
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, healthPointData);

            // 陣営とカテゴリのTagをアタッチ
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}
