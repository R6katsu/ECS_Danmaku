using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
#endif

// リファクタリング済み

/// <summary>
/// AxisTypeの補助
/// </summary>
static public class AxisTypeHelper
{
    /// <summary>
    /// AxisTypeに対応する各成分のfloat、例外はnullを返す
    /// </summary>
    /// <param name="axisType">軸の種類</param>
    /// <param name="position">成分を取り出すfloat3</param>
    /// <returns>AxisTypeに対応する各成分のfloat、例外はnull</returns>
    public static float GetAxisValue(AxisType axisType, float3 position)
    {
        switch (axisType)
        {
            case AxisType.X:
                return position.x;

            case AxisType.Y:
                return position.y;

            case AxisType.Z:
                return position.z;

            case AxisType.None:
            default:
#if UNITY_EDITOR
                Debug.LogError("AxisTypeが未設定、または例外");
#endif
                return 0.0f;
        }
    }

    /// <summary>
    /// AxisTypeに対応する方向、例外はnullを返す
    /// </summary>
    /// <param name="axisType">軸の種類</param>
    /// <returns>AxisTypeに対応する方向</returns>
    public static Vector3 GetAxisDirection(AxisType axisType)
    {
        switch (axisType)
        {
            case AxisType.X:
                return Vector3.right;

            case AxisType.Y:
                return Vector3.up;

            case AxisType.Z:
                return Vector3.forward;

            default:
#if UNITY_EDITOR
                Debug.LogError("AxisTypeが未設定、または例外");
#endif
                return Vector3.zero;
        }
    }
}
