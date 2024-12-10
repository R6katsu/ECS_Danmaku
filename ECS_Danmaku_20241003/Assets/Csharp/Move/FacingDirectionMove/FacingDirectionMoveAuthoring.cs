using Unity.Entities;
using UnityEngine;
using static MoveHelper;
using System;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static BulletHelper;
using static EntityCampsHelper;
using static EntityCategoryHelper;
#endif

// リファクタリング済み

/// <summary>
/// 向いている方向に移動する為の情報
/// </summary>
[Serializable]
public struct FacingDirectionMoveData : IComponentData
{
    [SerializeField, Header("移動に必要な設定値")]
    private MoveParameter _moveParameter;

    /// <summary>
    /// 移動に必要な設定値
    /// </summary>
    public MoveParameter MoveParameter => _moveParameter;

    /// <summary>
    /// 向いている方向に移動する為の情報
    /// </summary>
    /// <param name="moveParameter">移動に必要な設定値</param>
    public FacingDirectionMoveData(MoveParameter moveParameter)
    {
        _moveParameter = moveParameter;
    }
}

/// <summary>
/// 向いている方向に移動する為の設定
/// </summary>
public class FacingDirectionMoveAuthoring : MonoBehaviour
{
    [SerializeField, Header("向いている方向に移動する為の情報")]
    private FacingDirectionMoveData _facingDirectionMoveData = new();

    public class FacingDirectionMoveBaker : Baker<FacingDirectionMoveAuthoring>
    {
        public override void Bake(FacingDirectionMoveAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Dataをアタッチ
            AddComponent(entity, src._facingDirectionMoveData);
            AddComponent(entity, new MoveTag());
        }
    }
}
