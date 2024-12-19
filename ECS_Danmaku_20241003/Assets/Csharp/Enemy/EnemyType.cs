using UnityEngine;

#if UNITY_EDITOR
using Unity.Entities;
using System.Collections.Generic;
using Unity.Collections;
using static HealthHelper;
using static MoveHelper;
using NUnit.Framework;
using System.Security.Cryptography;
using Unity.Core;
#endif

// リファクタリング済み

/// <summary>
/// 敵Entityの種類
/// </summary>
public enum EnemyType : byte
{
    [Tooltip("全方位弾")] AllDirectionBulletEnemy,
    [Tooltip("回転撃ち")] SpinShootingEnemy,
}