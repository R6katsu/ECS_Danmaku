using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static DanmakuJobs;
using static HealthHelper;
using static PlayerAuthoring;
using static PlayerHelper;
using static TriggerHelper;
using static UnityEngine.EventSystems.EventTrigger; // if false‚ÍˆÓ–¡‚È‚©‚Á‚½‚Á‚Û‚¢
#if false
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
#endif

public struct PlayerData : IComponentData
{
    public readonly float moveSpeed;

    public PlayerData(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }
}

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField,Min(0.0f), Header("ˆÚ“®‘¬“x")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("Å‘å‘Ì—Í")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("–³“GŠÔ‚Ì’·‚³")]
    private float _isInvincibleTime = 0.0f;

    /// <summary>
    /// ˆÚ“®‘¬“x
    /// </summary>
    public float MoveSpeed => _moveSpeed;

    /// <summary>
    /// –³“GŠÔ‚Ì’·‚³
    /// </summary>
    public float IsInvincibleTime => _isInvincibleTime;

    /// <summary>
    /// Å‘å‘Ì—Í
    /// </summary>
    public float MaxHP => _maxHP;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerData(src.MoveSpeed));
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new PlayerHealthPointData(src.MaxHP, src.IsInvincibleTime));
        }
    }
}
