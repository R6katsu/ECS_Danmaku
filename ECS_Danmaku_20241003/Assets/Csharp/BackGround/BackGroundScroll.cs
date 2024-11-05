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

    /// <summary>
    /// 軸の種類
    /// </summary>
    private enum AxisType
    { X, Y, Z }

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
#if UNITY_EDITOR
        // ランタイム中ではなかった
        if (!Application.isPlaying) { return; }
#endif

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

        // 背景を前後に複製
        InstantiateBackGround("ForwardBackGround", transform.position + _backGroundSize.z * Vector3.forward, _backGround);
        InstantiateBackGround("BackBackGround", transform.position + _backGroundSize.z * Vector3.back, _backGround);

        // スクロール方向が反対だった
        if (_scrollSpeed < 0.0f)
        {
            // 親の名前を定義
            var parentName = "ParentBackScroll";

            // 親を作成
            var parentBackScroll = new GameObject();
            parentBackScroll.name = parentName;

            // 反対向きの定義
            var faceOppositeDirection = Vector3.up * 180.0f;

            // 親を設定
            transform.parent = parentBackScroll.transform;

            // 反対を向かせる
            parentBackScroll.transform.rotation = Quaternion.Euler(faceOppositeDirection);

            // 正の値に変更
            _scrollSpeed *= -1.0f;
        }

        // 初期位置
        _initialPosition = transform.position;

        // 初期位置から移動開始/終了位置までの距離を求める
        _endLength = _backGroundSize.z;

        // 移動方向
        switch (_axisType)
        {
            case AxisType.X:
                _moveDirection = Vector3.right * _scrollSpeed;
                _startPosition = _initialPosition + -_endLength * Vector3.right;
                break;

            case AxisType.Y:
                _moveDirection = Vector3.up * _scrollSpeed;
                _startPosition = _initialPosition + -_endLength * Vector3.up;
                break;

            case AxisType.Z:
                _moveDirection = Vector3.forward * _scrollSpeed;
                _startPosition = _initialPosition + -_endLength * Vector3.forward;
                break;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        // ランタイム中ではなかった
        if (!Application.isPlaying) { return; }
#endif

        // 今回のフレームでの移動量
        var frameMovement = Time.deltaTime * _moveDirection;

        // 目標に到達したか
        bool hasReachedTarget = false;

        switch (_axisType)
        {
            case AxisType.X:
                hasReachedTarget = _initialPosition.x + _endLength <= transform.localPosition.x + frameMovement.x;
                break;

            case AxisType.Y:
                hasReachedTarget = _initialPosition.y + _endLength <= transform.localPosition.y + frameMovement.y;
                break;

            case AxisType.Z:
                hasReachedTarget = _initialPosition.z + _endLength <= transform.localPosition.z + frameMovement.z;
                break;
        }

        // 目標に到達した
        if (hasReachedTarget)
        {
            // 移動開始位置に移動
            transform.localPosition = _startPosition;
        }
        else
        {
            // 移動を反映
            transform.localPosition += frameMovement;
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
