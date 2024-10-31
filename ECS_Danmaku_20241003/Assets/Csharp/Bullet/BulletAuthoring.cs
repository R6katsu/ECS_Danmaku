using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static BulletHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// 弾の情報
/// </summary>
public struct BulletData : IComponentData
{
    [Tooltip("寿命")]
    public readonly float lifeTime;

    [Tooltip("経過時間")]
    public float elapsed;

    /// <summary>
    /// 弾の情報
    /// </summary>
    /// <param name="lifeTime">寿命</param>
    public BulletData(float lifeTime)
    {
        this.lifeTime = lifeTime;
        elapsed = 0.0f;
    }
}

/// <summary>
/// 弾の設定
/// </summary>
public class BulletAuthoring : MonoBehaviour
{
    // なぜ、毎回BulletAuthoringだけが読み込まれなくなる？
    // 処理をコピペして他のBakeに貼り付けたところ機能した
    // つまり処理内容に不備があり、途中で止まったり、読み込まれなかった訳ではない

    // オリジナルのみ接触した時にダメージを受けた。つまり、動的にインスタンスから生成したものとは違う
    // Prefabに事前に設定しておき、Bakeで取り付けるのではなく、生成時に生成側から必要なBakeをする？
    // いや、BakeはObjectが必要な筈。
    // また、オリジナルに関しても接触時に体力が減ったもののエラーが発生した
    // いや、これは違う。悪くない。オリジナルに接触したことでオリジナルが消えて生成時に参照できないだけ



    [Tooltip("無制限に貫通する下限")]
    private const int UNLIMITED_PIERCE_MINIMUM = -1;

    [SerializeField, Min(0.0f), Header("ダメージ量")]
    private float _damageAmount = 0.0f;

    [SerializeField, Min(0.0f), Header("寿命")]
    private float _lifeTime = 0.0f;

    [SerializeField, Min(UNLIMITED_PIERCE_MINIMUM), Header("残り貫通回数（下限 = 無限）")]
    private int _remainingPierceCount = 0;

    [SerializeField, Header("陣営の種類")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entityのカテゴリ")]
    private EntityCategory _entityCategory = 0;

    public class BulletBaker : Baker<BulletAuthoring>
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
            AddComponent(entity, new BulletData(src._lifeTime));
            AddComponent(entity, new BulletIDealDamageData(src._damageAmount, src._campsType));

            // 陣営とカテゴリのTagをアタッチ
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCategoryTagType(src._entityCategory));
        }
    }
}
