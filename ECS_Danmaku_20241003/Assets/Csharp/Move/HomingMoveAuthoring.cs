using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static EntityCampsHelper;
using static EntityCategoryHelper;
using static MoveHelper;

/// <summary>
/// 対象に向かって直線移動する為の情報
/// </summary>
public struct HomingMoveData : IComponentData
{
    public readonly MoveParameter moveParam;

    [Tooltip("目標の陣営")]
    public readonly EntityCampsType targetCampsType;

    [Tooltip("目標のカテゴリ")]
    public readonly EntityCategory targetEntityCategory;

    public Entity targetEntity;

    /// <summary>
    /// 対象に向かって直線移動する為の情報
    /// </summary>
    /// <param name="moveParam">移動の設定値</param>
    /// <param name="targetCampsType">目標の陣営</param>
    /// <param name="targetEntityCategory">目標のカテゴリ</param>
    /// <param name="targetEntity">目標のEntity</param>
    public HomingMoveData(MoveParameter moveParam,　EntityCampsType targetCampsType, EntityCategory targetEntityCategory, Entity targetEntity = new Entity())
    {
        this.moveParam = moveParam;
        this.targetCampsType = targetCampsType;
        this.targetEntityCategory = targetEntityCategory;
        this.targetEntity = targetEntity;
    }
}

public class HomingMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField, Header("目標の陣営")]
    private EntityCampsType _targetCampsType = 0;

    [SerializeField, Header("目標のカテゴリ")]
    private EntityCategory _targetEntityCategory = 0;

    /// <summary>
    /// 移動の設定値
    /// </summary>
    public MoveParameter MyMoveParam => _moveParam;

    /// <summary>
    /// 目標の陣営
    /// </summary>
    public EntityCampsType TargetCampsType => _targetCampsType;

    /// <summary>
    /// 目標のカテゴリ
    /// </summary>
    public EntityCategory TargetEntityCategory => _targetEntityCategory;

    public class Baker : Baker<HomingMoveAuthoring>
    {
        public override void Bake(HomingMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 対象に向かって直線移動する為の情報を作成
            var homingMove = new HomingMoveData(src.MyMoveParam, src.TargetCampsType, src.TargetEntityCategory);

            // Dataをアタッチ
            AddComponent(entity, homingMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
