using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;

#if UNITY_EDITOR
using Unity.Physics;
#endif

/// <summary>
/// 背景スクロール
/// </summary>
public class BackGroundScroll : MonoBehaviour
{
    /// <summary>
    /// 軸の種類
    /// </summary>
    public enum AxisType
    { X, Y, Z }

    [SerializeField, Header("スクロールさせる方向")]
    private AxisType _axisType = 0;

    [SerializeField, Header("スクロールさせる背景")]
    private Transform _backGround = null;

    [SerializeField, Header("目標までの長さ")]
    private float _targetLength = 0.0f;

    [SerializeField, Header("スクロール速度")]
    private float _scrollSpeed = 0.0f;

    [Tooltip("スクロールさせる方向の軸の大きさ")]
    private float _axisScale = 0.0f;

    [Tooltip("移動方向")]
    private Vector3 _moveDirection = Vector3.zero;

    [Tooltip("初期位置")]
    private Vector3 _initialPosition = Vector3.zero;

    private float _loopTargetLength = 0.0f;

    private void OnEnable()
    {
        // スクロールさせる背景がnullだった
        if (_backGround == null)
        {
            enabled = false;
#if UNITY_EDITOR
            Debug.LogError("スクロールさせる背景がnullだった");
#endif
            return;
        }

        // スクロール方向が反対だった
        if (_scrollSpeed < 0.0f)
        {
            // 親の名前を定義
            var parentName = "ParentBackScroll";

            // 親を作成
            var parentBackScroll = new GameObject();
            parentBackScroll.name = parentName;

            // 反対の定義
            var faceOppositeDirection = Vector3.up * 180.0f;

            // 親を設定
            _backGround.transform.parent = parentBackScroll.transform;

            // 反対を向かせる
            parentBackScroll.transform.rotation = Quaternion.Euler(faceOppositeDirection);

            // 正の値に変更
            _scrollSpeed *= -1.0f;
        }

        _initialPosition = _backGround.transform.position;

        _loopTargetLength = -_targetLength;

        switch (_axisType)
        {
            case AxisType.X:
                _axisScale = _backGround.localScale.x;
                _moveDirection = Vector3.right * _scrollSpeed;
                break;

            case AxisType.Y:
                _axisScale = _backGround.localScale.y;
                _moveDirection = Vector3.up * _scrollSpeed;
                break;

            case AxisType.Z:
                _axisScale = _backGround.localScale.z;
                _moveDirection = Vector3.forward * _scrollSpeed;
                break;
        }
    }

    private void Update()
    {
        var aaa = Time.deltaTime * _moveDirection;

        bool aajioa = false;
        Vector3 gafaaa = Vector3.zero;

        switch (_axisType)
        {
            case AxisType.X:
                aajioa = _initialPosition.x + _targetLength <= _backGround.transform.localPosition.x + aaa.x;
                gafaaa = _initialPosition + _loopTargetLength * Vector3.right;
                break;

            case AxisType.Y:
                aajioa = _initialPosition.y + _targetLength <= _backGround.transform.localPosition.y + aaa.y;
                gafaaa = _initialPosition + _loopTargetLength * Vector3.up;
                break;

            case AxisType.Z:
                aajioa = _initialPosition.z + _targetLength <= _backGround.transform.localPosition.z + aaa.z;
                gafaaa = _initialPosition + _loopTargetLength * Vector3.forward;
                break;
        }

        if (aajioa)
        {
            _backGround.transform.localPosition = gafaaa;
        }
        else
        {
            _backGround.transform.localPosition += aaa;
        }
    }
}
