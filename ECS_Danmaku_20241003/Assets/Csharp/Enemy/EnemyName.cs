using UnityEngine;

/// <summary>
/// 敵Entityの名称
/// </summary>
public enum EnemyName : byte
{
    [Tooltip("")] Enemy,
    [Tooltip("")] TapShooting_Rotation,
    [Tooltip("パターン終了まで余白を作る為の敵モドキ")] DummyEnemy,
}
