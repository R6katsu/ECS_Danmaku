using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// íœ‚ÉŠÖ‚·‚éî•ñ
/// </summary>
public struct DestroyableData : IComponentData
{
    [Tooltip("íœƒtƒ‰ƒO")]
    public bool isKilled;
}
