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
using static EntityCampsHelper;

using static EntityCategoryHelper;
using static HealthPointDatas;


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
    public readonly float slowMoveSpeed;
    public readonly float firingInterval;
    public readonly Entity playerBulletEntity;

    public double lastShotTime;
    public bool isSlowdown;

    /// <summary>
    /// PLの情報
    /// </summary>
    /// <param name="moveSpeed">移動速度</param>
    /// <param name="moveSlowSpeed">減速中の移動速度</param>
    /// <param name="firingInterval">射撃間隔</param>
    /// <param name="playerBulletEntity">PLの弾のEntity</param>
    public PlayerData(float moveSpeed, float moveSlowSpeed, float firingInterval, Entity playerBulletEntity)
    {
        this.moveSpeed = moveSpeed;
        this.slowMoveSpeed = moveSlowSpeed;
        this.firingInterval = firingInterval;
        this.playerBulletEntity = playerBulletEntity;

        lastShotTime = 0.0f;
        isSlowdown = false;
    }
}

/// <summary>
/// PLの設定
/// </summary>
public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField,Min(0.0f), Header("移動速度")]
    private float _moveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("減速中の移動速度")]
    private float _slowMoveSpeed = 0.0f;

    [SerializeField, Min(0.0f), Header("最大体力")]
    private float _maxHP = 0.0f;

    [SerializeField, Min(0.0f), Header("射撃間隔")]
    private float _firingInterval = 0.0f;

    [SerializeField, Min(0.0f), Header("無敵時間の長さ")]
    private float _isInvincibleTime = 0.0f;

    [SerializeField, Header("PLの弾のPrefab")]
    private Transform _playerBulletPrefab = null;

    [SerializeField, Header("陣営の種類")]
    private EntityCampsType _campsType = 0;

    [SerializeField, Header("Entityのカテゴリ")]
    private EntityCategory _entityCategory = 0;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var PlayerBulletEntity = GetEntity(src._playerBulletPrefab, TransformUsageFlags.Dynamic);

            // Dataをアタッチ
            AddComponent(entity, new PlayerData(src._moveSpeed, src._slowMoveSpeed, src._firingInterval, PlayerBulletEntity));
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DestroyableData());
            AddComponent(entity, new PlayerHealthPointData(src._maxHP, src._isInvincibleTime));

            // 陣営とカテゴリのTagをアタッチ
            AddComponent(entity, EntityCampsHelper.GetCampsTagType(src._campsType));
            AddComponent(entity, EntityCategoryHelper.GetCampsTagType(src._entityCategory));
        }
    }
}
