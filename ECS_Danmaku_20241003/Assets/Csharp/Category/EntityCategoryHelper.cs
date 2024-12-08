using Unity.Entities;
using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static EntityCampsHelper;
#endif

// リファクタリング済み

/// <summary>
/// Entityのカテゴリ
/// </summary>
public enum EntityCategory : byte
{
    [Tooltip("不明")] Unknown,
    [Tooltip("破壊不可")] Indestructible,
    [Tooltip("武器")] Weapon,
    [Tooltip("弾幕")] Danmaku,
    [Tooltip("生物")] LivingCreature
}

/// <summary>
/// Entityのカテゴリの補助
/// </summary>
static public class EntityCategoryHelper
{
    /// <summary>
    /// EntityCategoryに応じたTagの型を返す
    /// </summary>
    /// <param name="entityCategory">Entityのカテゴリの種類</param>
    /// <returns>対応するTagの型</returns>
    static public Type GetCategoryTagType(EntityCategory entityCategory)
    {
        switch (entityCategory)
        {
            case EntityCategory.Weapon:
                return typeof(WeaponCategoryTag);

            case EntityCategory.Danmaku:
                return typeof(DanmakuCategoryTag);

            case EntityCategory.LivingCreature:
                return typeof(LivingCreatureCategoryTag);

            case EntityCategory.Indestructible:
            default:
                return typeof(IndestructibleCategoryCampsTag);
        }
    }

    /// <summary>
    /// Entityのカテゴリの情報
    /// </summary>
    public struct EntityCategoryData : IComponentData
    {
        public readonly EntityCategory entityCategory;

        /// <summary>
        /// Entityのカテゴリの情報
        /// </summary>
        /// <param name="entityCategory">Entityのカテゴリ</param>
        public EntityCategoryData(EntityCategory entityCategory)
        {
            this.entityCategory = entityCategory;
        }
    }

    /// <summary>
    /// 破壊不可カテゴリ
    /// </summary>
    public struct IndestructibleCategoryCampsTag : IComponentData { }

    /// <summary>
    /// 武器カテゴリ
    /// </summary>
    public struct WeaponCategoryTag : IComponentData { }

    /// <summary>
    /// 弾幕カテゴリ
    /// </summary>
    public struct DanmakuCategoryTag : IComponentData { }

    /// <summary>
    /// 生物カテゴリ
    /// </summary>
    public struct LivingCreatureCategoryTag : IComponentData { }
}
