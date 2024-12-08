using Unity.Entities;
using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using static EntityCampsHelper;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// Entity�̃J�e�S��
/// </summary>
public enum EntityCategory : byte
{
    [Tooltip("�s��")] Unknown,
    [Tooltip("�j��s��")] Indestructible,
    [Tooltip("����")] Weapon,
    [Tooltip("�e��")] Danmaku,
    [Tooltip("����")] LivingCreature
}

/// <summary>
/// Entity�̃J�e�S���̕⏕
/// </summary>
static public class EntityCategoryHelper
{
    /// <summary>
    /// EntityCategory�ɉ�����Tag�̌^��Ԃ�
    /// </summary>
    /// <param name="entityCategory">Entity�̃J�e�S���̎��</param>
    /// <returns>�Ή�����Tag�̌^</returns>
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
    /// Entity�̃J�e�S���̏��
    /// </summary>
    public struct EntityCategoryData : IComponentData
    {
        public readonly EntityCategory entityCategory;

        /// <summary>
        /// Entity�̃J�e�S���̏��
        /// </summary>
        /// <param name="entityCategory">Entity�̃J�e�S��</param>
        public EntityCategoryData(EntityCategory entityCategory)
        {
            this.entityCategory = entityCategory;
        }
    }

    /// <summary>
    /// �j��s�J�e�S��
    /// </summary>
    public struct IndestructibleCategoryCampsTag : IComponentData { }

    /// <summary>
    /// ����J�e�S��
    /// </summary>
    public struct WeaponCategoryTag : IComponentData { }

    /// <summary>
    /// �e���J�e�S��
    /// </summary>
    public struct DanmakuCategoryTag : IComponentData { }

    /// <summary>
    /// �����J�e�S��
    /// </summary>
    public struct LivingCreatureCategoryTag : IComponentData { }
}
