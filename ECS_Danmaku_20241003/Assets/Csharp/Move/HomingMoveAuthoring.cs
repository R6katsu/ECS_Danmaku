using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static MoveHelper;

/// <summary>
/// 対象に向かって直線移動する為の情報
/// </summary>
public struct HomingMoveData : IComponentData
{
    public readonly MoveParameter moveParam;
    public readonly CampsType campsType;
    public Entity targetEntity;

    public HomingMoveData(MoveParameter moveParam, CampsType campsType, Entity targetPoint = new Entity())
    {
        this.moveParam = moveParam;
        this.campsType = campsType;
        this.targetEntity = targetPoint;
    }
}

public class HomingMoveAuthoring : MonoBehaviour
{
    [SerializeField]
    private MoveParameter _moveParam = new();

    [SerializeField]
    private CampsType _campsType = 0;

    /// <summary>
    /// 移動の設定値
    /// </summary>
    public MoveParameter MyMoveParam => _moveParam;

    /// <summary>
    /// 陣営の種類
    /// </summary>
    public CampsType CampsType => _campsType;

    public class Baker : Baker<HomingMoveAuthoring>
    {
        public override void Bake(HomingMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // 対象に向かって直線移動する為の情報を作成
            var homingMove = new HomingMoveData(src.MyMoveParam, src.CampsType);

            // Dataをアタッチ
            AddComponent(entity, homingMove);
            AddComponent(entity, new MoveTag());
        }
    }
}
