using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static HealthHelper;
using static MoveHelper;

#if UNITY_EDITOR
using NUnit.Framework;
using System.Security.Cryptography;
using Unity.Core;
#endif

/// <summary>
/// ìGÇÃï‚èï
/// </summary>
static public class EnemyHelper
{
    /// <summary>
    /// ìG
    /// </summary>
    public struct EnemyTag : IComponentData{ }
}
