using UnityEngine;


#if UNITY_EDITOR
using Unity.Physics;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
#endif

/// <summary>
/// 背景スクロール
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class BackGroundScroll : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // 黄色
        Gizmos.color = Color.yellow;

        // 複製した背景の出現位置
        Gizmos.DrawWireCube(transform.position + _backGroundSize.z * Vector3.forward, _backGroundSize);
        Gizmos.DrawWireCube(transform.position + _backGroundSize.z * Vector3.back, _backGroundSize);

        // 緑
        Gizmos.color = Color.green;

        // オリジナルの背景の大きさ
        Gizmos.DrawWireCube(transform.position, _backGroundSize);
    }

    [SerializeField, Header("スクロールさせる方向")]
    private AxisType _axisType = 0;

    [SerializeField, Header("スクロールさせる背景")]
    private Transform _backGround = null;

    [SerializeField, Header("スクロール速度")]
    private float _scrollSpeed = 0.0f;

    [SerializeField, Header("背景の大きさ")]
    private Vector3 _backGroundSize = Vector3.zero;

    [Tooltip("移動方向")]
    private Vector3 _moveDirection = Vector3.zero;

    [Tooltip("初期位置")]
    private Vector3 _initialPosition = Vector3.zero;

    [Tooltip("移動開始位置")]
    private Vector3 _startPosition = Vector3.zero;

    [Tooltip("初期位置から移動終了位置までの距離")]
    private float _endLength = 0.0f;

    private void OnEnable()
    {
        // 背景がnullだった
        if (_backGround == null)
        {
#if UNITY_EDITOR
            // 新しく背景を生成する
            _backGround = InstantiateBackGround("BackGround", Vector3.zero);
#else
            enabled = false;
            return;
#endif
        }

#if UNITY_EDITOR
        // ランタイム中ではなかった
        if (!Application.isPlaying) { return; }
#endif

        // 背景を前後に複製
        InstantiateBackGround("ForwardBackGround", transform.position + _backGroundSize.z * Vector3.forward, _backGround);
        InstantiateBackGround("BackBackGround", transform.position + _backGroundSize.z * Vector3.back, _backGround);

        // 初期位置
        _initialPosition = transform.position;

        // 初期位置から移動開始/終了位置までの距離を求める
        _endLength = _backGroundSize.z;

        // AxisTypeに対応する方向を取得
        var axisValue = AxisTypeHelper.GetAxisDirection(_axisType);

        // 方向と速度
        _moveDirection = axisValue * _scrollSpeed;

        // 開始位置
        Vector3 startPosition = -_endLength * axisValue;

        // 目標と開始位置の符号を反対にする
        _startPosition = (_scrollSpeed < 0.0f) ? _initialPosition - startPosition : _initialPosition + startPosition;
    }

    private void Update()
    {
#if UNITY_EDITOR
        // ランタイム中ではなかった
        if (!Application.isPlaying) { return; }
#endif
        var endLength = _endLength;

        // 今回のフレームでの移動量
        var frameMovement = Time.deltaTime * _moveDirection;

        // 目標に到達したか
        bool hasReachedTarget = false;

        // 移動方向が反転しているか
        bool isMovingBackwards = _scrollSpeed < 0.0f;

        // 反転していたら符号を反転させる
        endLength *= (isMovingBackwards) ? -1.0f : 1.0f;

        // AxisTypeに対応する各成分を取得
        var initialPositionValue = AxisTypeHelper.GetAxisValue(_axisType, _initialPosition);
        var localPositionValue = AxisTypeHelper.GetAxisValue(_axisType, transform.position);
        var frameMovementValue = AxisTypeHelper.GetAxisValue(_axisType, frameMovement);

        // 移動方向によって終了距離と現在の移動距離を変える
        float end = (isMovingBackwards) ? initialPositionValue + endLength : localPositionValue + frameMovementValue;
        float current = (isMovingBackwards) ? localPositionValue + frameMovementValue : initialPositionValue + endLength;

        // 目標に到達したか
        hasReachedTarget = end >= current;

        // 目標に到達した
        if (hasReachedTarget)
        {
            // 移動開始位置に移動
            transform.position = _startPosition;
        }
        else
        {
            // 移動を反映
            transform.position += frameMovement;
        }
    }

    /// <summary>
    /// 背景を生成する
    /// </summary>
    /// <param name="name">生成、または複製した対象の名前</param>
    /// <param name="position">生成位置</param>
    /// <param name="original">複製する場合のオリジナル</param>
    /// <returns>生成、または複製した対象</returns>
    private Transform InstantiateBackGround(string name, Vector3 position, Transform original = null)
    {
        // 生成対象がnullだったら新しく生成、違えば複製
        var backGround = (original == null) ? new GameObject().transform : Instantiate(original);

        // 初期設定
        backGround.name = name;
        backGround.transform.parent = transform;
        backGround.localPosition = position;

        return backGround;
    }
}
