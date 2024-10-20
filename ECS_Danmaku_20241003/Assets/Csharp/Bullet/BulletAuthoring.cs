using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;

public class BulletAuthoring : MonoBehaviour
{
    [Tooltip("無制限に貫通する下限")]
    private const int UNLIMITED_PIERCE_MINIMUM = -1;

    [SerializeField, Min(0.0f), Header("ダメージ量")]
    private float _damageAmount = 0.0f;

    [SerializeField, Min(UNLIMITED_PIERCE_MINIMUM), Header("残り貫通回数（下限 = 無限）")]
    private int _remainingPierceCount = 0;

    [SerializeField, Header("陣営の種類")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entityのカテゴリ")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 残り貫通回数が下限ではなかった
            if (src._remainingPierceCount != BulletAuthoring.UNLIMITED_PIERCE_MINIMUM)
            {
                // 残り貫通回数のDataをアタッチ
                AddComponent(entity, new RemainingPierceCountData(src._remainingPierceCount));
            }

            AddComponent(entity, new BulletTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new BulletIDealDamageData(src._damageAmount, src._campsType));

            // 陣営とカテゴリのTagをアタッチ
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCampsTagType(src._entityCategory));

            // 必要のなくなったAuthoringを無効化
            //src.enabled = false;
        }
    }
}
