using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static PlayerHelper;

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
    [SerializeField,Min(0.0f), Header("移動速度")]
    private float _moveSpeed = 0.0f;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float MoveSpeed => _moveSpeed;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PlayerData(src.MoveSpeed));
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PlayerTag());
        }
    }
}
