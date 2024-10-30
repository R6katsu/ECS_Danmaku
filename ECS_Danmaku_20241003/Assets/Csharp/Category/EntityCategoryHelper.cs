using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using UnityEngine;

#if UNITY_EDITOR
using static EntityCampsHelper;
#endif

/// <summary>
/// Entity‚ÌƒJƒeƒSƒŠ‚Ì•â•
/// </summary>
static public class EntityCategoryHelper
{
    /// <summary>
    /// EntityCategory‚É‰‚¶‚½Tag‚ÌŒ^‚ğ•Ô‚·
    /// </summary>
    /// <param name="entityCategory">Entity‚ÌƒJƒeƒSƒŠ‚Ìí—Ş</param>
    /// <returns>‘Î‰‚·‚éTag‚ÌŒ^</returns>
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
    /// Entity‚ÌƒJƒeƒSƒŠ
    /// </summary>
    public enum EntityCategory
    {
        Unknown,
        [Tooltip("”j‰ó•s‰Â")] Indestructible,
        [Tooltip("•Ší")] Weapon,
        [Tooltip("’e–‹")] Danmaku,
        [Tooltip("¶•¨")] LivingCreature
    }

    /// <summary>
    /// Entity‚ÌƒJƒeƒSƒŠ‚Ìî•ñ
    /// </summary>
    public struct EntityCategoryData : IComponentData
    {
        public EntityCategory myEntityCategory;

        /// <summary>
        /// Entity‚ÌƒJƒeƒSƒŠ‚Ìî•ñ
        /// </summary>
        /// <param name="EntityCategory">Entity‚ÌƒJƒeƒSƒŠ</param>
        public EntityCategoryData(EntityCategory EntityCategory)
        {
            this.myEntityCategory = EntityCategory;
        }
    }

    /// <summary>
    /// ”j‰ó•s‰ÂƒJƒeƒSƒŠ
    /// </summary>
    public struct IndestructibleCategoryCampsTag : IComponentData { }

    /// <summary>
    /// •ŠíƒJƒeƒSƒŠ
    /// </summary>
    public struct WeaponCategoryTag : IComponentData { }

    /// <summary>
    /// ’e–‹ƒJƒeƒSƒŠ
    /// </summary>
    public struct DanmakuCategoryTag : IComponentData { }

    /// <summary>
    /// ¶•¨ƒJƒeƒSƒŠ
    /// </summary>
    public struct LivingCreatureCategoryTag : IComponentData { }
}
