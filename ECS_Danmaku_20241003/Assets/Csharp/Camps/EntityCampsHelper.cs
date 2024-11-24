using Unity.Entities;
using System;
using UnityEngine;

/// <summary>
/// Entity‚Ìw‰c‚Ì•â•
/// </summary>
static public class EntityCampsHelper
{
    /// <summary>
    /// CampsType‚É‰‚¶‚½Tag‚ÌŒ^‚ğ•Ô‚·
    /// </summary>
    /// <param name="campsType">w‰c‚Ìí—Ş</param>
    /// <returns>‘Î‰‚·‚éTag‚ÌŒ^</returns>
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
    /// Entity‚Ìw‰c‚Ìí—Ş
    /// </summary>
    public enum EntityCampsType
    {
        Unknown,
        Enemy,
        Player,
        BossEnemy
    }

    /// <summary>
    /// ©w‰c‚Ìí—Ş‚Ìî•ñ
    /// </summary>
    public struct EntityCampsData : IComponentData
    {
        public EntityCampsType myCampsType;

        /// <summary>
        /// ©w‰c‚Ìí—Ş‚Ìî•ñ
        /// </summary>
        /// <param name="myCampsType">w‰c‚Ìí—Ş</param>
        public EntityCampsData(EntityCampsType myCampsType)
        {
            this.myCampsType = myCampsType;
        }
    }

    /// <summary>
    /// w‰c•s–¾
    /// </summary>
    public struct UnknownCampsTag : IComponentData { }

    /// <summary>
    /// “Gw‰c
    /// </summary>
    public struct EnemyCampsTag : IComponentData { }

    /// <summary>
    /// PLw‰c
    /// </summary>
    public struct PlayerCampsTag : IComponentData { }

    /// <summary>
    /// ƒ{ƒX“Gw‰c
    /// </summary>
    public struct BossEnemyCampsTag : IComponentData { }
}