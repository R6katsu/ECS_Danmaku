using Unity.Entities;
using System;
using UnityEngine;

/// <summary>
/// Entity�̐w�c�̕⏕
/// </summary>
static public class EntityCampsHelper
{
    /// <summary>
    /// CampsType�ɉ�����Tag�̌^��Ԃ�
    /// </summary>
    /// <param name="campsType">�w�c�̎��</param>
    /// <returns>�Ή�����Tag�̌^</returns>
    static public Type GetCampsTagType(EntityCampsType campsType)
    {
        switch (campsType)
        {
            case EntityCampsType.Enemy:
                return typeof(EnemyCampsTag);

            case EntityCampsType.Player:
                return typeof(PlayerCampsTag);

            case EntityCampsType.BossEnemy:
                return typeof(BossEnemyCampsTag);

            case EntityCampsType.Unknown:
            default:
                return typeof(UnknownCampsTag);
        }
    }

    /// <summary>
    /// Entity�̐w�c�̎��
    /// </summary>
    public enum EntityCampsType
    {
        Unknown,
        Enemy,
        Player,
        BossEnemy
    }

    /// <summary>
    /// ���w�c�̎�ނ̏��
    /// </summary>
    public struct EntityCampsData : IComponentData
    {
        public EntityCampsType myCampsType;

        /// <summary>
        /// ���w�c�̎�ނ̏��
        /// </summary>
        /// <param name="myCampsType">�w�c�̎��</param>
        public EntityCampsData(EntityCampsType myCampsType)
        {
            this.myCampsType = myCampsType;
        }
    }

    /// <summary>
    /// �w�c�s��
    /// </summary>
    public struct UnknownCampsTag : IComponentData { }

    /// <summary>
    /// �G�w�c
    /// </summary>
    public struct EnemyCampsTag : IComponentData { }

    /// <summary>
    /// PL�w�c
    /// </summary>
    public struct PlayerCampsTag : IComponentData { }

    /// <summary>
    /// �{�X�G�w�c
    /// </summary>
    public struct BossEnemyCampsTag : IComponentData { }
}