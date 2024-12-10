using Unity.Entities;

#if UNITY_EDITOR
using UnityEngine;
using static HealthHelper;
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// PLの補助
/// </summary>
static public class PlayerHelper
{
    /// <summary>
    /// Player
    /// </summary>
    public struct PlayerTag : IComponentData { }
}
