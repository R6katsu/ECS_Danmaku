using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static DanmakuJobs;
using static HealthHelper;
using static PlayerAuthoring;
using static PlayerHelper;
using static TriggerHelper;
#if UNITY_EDITOR
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
#endif

/// <summary>
/// PLの情報
/// </summary>
public struct PlayerData : IComponentData
{
    public readonly float moveSpeed;

    /// <summary>
    /// PLの情報
    /// </summary>
    /// <param name="moveSpeed">移動速度</param>
    public PlayerData(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }
}

/// <summary>
/// PLの設定
/// </summary>
public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField,Min(0.0f), Header("移動速度")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("最大体力")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("無敵時間の長さ")]
    private float _isInvincibleTime = 0.0f;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float MoveSpeed => _moveSpeed;

    /// <summary>
    /// 無敵時間の長さ
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// 最大体力
    /// </summary>
    public float MaxHP => _maxHP;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Dataをアタッチ
            AddComponent(entity, new PlayerData(src.MoveSpeed));
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new PlayerHealthPointData(src.MaxHP, src.IsInvincibleTime));
        }
    }
}
