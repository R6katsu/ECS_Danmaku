using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// AxisType�̕⏕
/// </summary>
static public class AxisTypeHelper
{
    /// <summary>
    /// AxisType�ɑΉ�����e������float�A��O��null��Ԃ�
    /// </summary>
    /// <param name="axisType">���̎��</param>
    /// <param name="position">���������o��float3</param>
    /// <returns>AxisType�ɑΉ�����e������float�A��O��null</returns>
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
                Debug.LogError("AxisType�����ݒ�A�܂��͗�O");
#endif
                return 0.0f;
        }
    }

    /// <summary>
    /// AxisType�ɑΉ���������A��O��null��Ԃ�
    /// </summary>
    /// <param name="axisType">���̎��</param>
    /// <returns>AxisType�ɑΉ��������</returns>
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
                Debug.LogError("AxisType�����ݒ�A�܂��͗�O");
#endif
                return Vector3.zero;
        }
    }
}
