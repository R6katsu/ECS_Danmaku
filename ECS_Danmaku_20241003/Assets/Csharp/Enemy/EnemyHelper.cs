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

// t@N^OΟέ

/// <summary>
/// GEntityΜΌΜ
/// </summary>
public enum EnemyName : byte
{
    [Tooltip("SϋΚe")] AllDirectionBulletEnemy,
    [Tooltip("ρ]Ώ")] SpinShootingEnemy,
}

/// <summary>
/// GΜβ
/// </summary>
static public class EnemyHelper
{
    /// <summary>
    /// G
    /// </summary>
    public struct EnemyTag : IComponentData{ }
}
