using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Collections;
using static HealthHelper;
using static MoveHelper;
using NUnit.Framework;
using System.Security.Cryptography;
using Unity.Core;
#endif

// ƒŠƒtƒ@ƒNƒ^ƒŠƒ“ƒOÏ‚İ

/// <summary>
/// “GEntity‚Ì–¼Ì
/// </summary>
public enum EnemyName : byte
{
    [Tooltip("‘S•ûˆÊ’e")] AllDirectionBulletEnemy,
    [Tooltip("‰ñ“]Œ‚‚¿")] SpinShootingEnemy,
}

/// <summary>
/// “G‚Ì•â•
/// </summary>
static public class EnemyHelper
{
    /// <summary>
    /// “G
    /// </summary>
    public struct EnemyTag : IComponentData{ }
}
