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
using static UnityEngine.EventSystems.EventTrigger; // if false�͈Ӗ��Ȃ��������ۂ�
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
    [SerializeField,Min(0.0f), Header("�ړ����x")]
    private float _moveSpeed = 0.0f;

    /// <summary>
    /// �ړ����x
    /// </summary>
    public float MoveSpeed => _moveSpeed;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerData(src.MoveSpeed));
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new PlayerHealthPointData());
        }
    }
}
