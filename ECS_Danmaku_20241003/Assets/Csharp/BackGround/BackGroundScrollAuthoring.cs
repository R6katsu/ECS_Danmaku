using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using Child = Unity.Transforms.Child;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using Unity.VisualScripting;



#if UNITY_EDITOR
using static BulletHelper;
#endif

/// <summary>
/// 背景スクロールの情報
/// </summary>
public struct BackGroundScrollData : IComponentData
{
    [Tooltip("スクロールさせる方向")]
    public readonly AxisType axisType;

    [Tooltip("移動方向")]
    public readonly float3 moveDirection;

    [Tooltip("初期位置")]
    public readonly float3 initialPosition;

    [Tooltip("移動開始位置")]
    public readonly float3 startPosition;

    [Tooltip("初期位置から移動終了位置までの距離")]
    public readonly float endLength;

    /// <summary>
    /// 背景スクロールの情報
    /// </summary>
    /// <param name="axisType">スクロールさせる方向</param>
    /// <param name="moveDirection">移動方向</param>
    /// <param name="initialPosition">初期位置</param>
    /// <param name="startPosition">移動開始位置</param>
    /// <param name="endLength">初期位置から移動終了位置までの距離</param>
    public BackGroundScrollData
        (
            AxisType axisType,
            float3 moveDirection,
            float3 initialPosition,
            float3 startPosition,
            float endLength
        )
    {
        this.axisType = axisType;
        this.moveDirection = moveDirection;
        this.initialPosition = initialPosition;
        this.startPosition = startPosition;
        this.endLength = endLength;
    }
}

/// <summary>
/// 背景スクロールの設定
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class BackGroundScrollAuthoring : MonoBehaviour
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

    [SerializeField, Header("スクロール速度")]
    private float _scrollSpeed = 0.0f;

    [SerializeField, Header("背景の大きさ")]
    private Vector3 _backGroundSize = Vector3.zero;

    [SerializeField, Header("スクロールさせる背景")]
    private Transform _backGround = null;

    [SerializeField, Header("前方に複製した背景")]
    private Transform _forwardBackGround = null;

    [SerializeField, Header("後方に複製した背景")]
    private Transform _backBackGround = null;

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
        var scrollSpeed = _scrollSpeed;

        // 背景がnullだった
        if (_backGround == null)
        {
#if UNITY_EDITOR
            // 新しく背景を生成
            _backGround = InstantiateBackGround("BackGround", Vector3.zero);
#else
            enabled = false;
            return;
#endif
        }

#if UNITY_EDITOR
        if (_forwardBackGround == null)
        {
            // 背景を前方に複製
            _forwardBackGround = InstantiateBackGround("ForwardBackGround", transform.position + _backGroundSize.z * Vector3.forward, _backGround);
        }
        if (_backBackGround == null)
        {
            // 背景を後方に複製
            _backBackGround = InstantiateBackGround("BackBackGround", transform.position + _backGroundSize.z * Vector3.back, _backGround);
        }
#else
            enabled = false;
            return;
#endif
        // 初期位置
        _initialPosition = transform.position;

        // 初期位置から移動開始/終了位置までの距離を求める
        _endLength = _backGroundSize.z;

        // AxisTypeに対応する方向を取得
        var axisValue = AxisTypeHelper.GetAxisDirection(_axisType);

        // 方向と速度
        _moveDirection = axisValue * scrollSpeed;

         // 開始位置
        Vector3 startPosition = -_endLength * axisValue;

        // 目標と開始位置の符号を反対にする
        _startPosition = (scrollSpeed < 0.0f) ? _initialPosition - startPosition : _initialPosition + startPosition;
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

    public class Baker : Baker<BackGroundScrollAuthoring>
    {
        public override void Bake(BackGroundScrollAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var backGroundScrollData = new BackGroundScrollData
                (
                    src._axisType,
                    src._moveDirection,
                    src._initialPosition,
                    src._startPosition,
                    src._endLength
                );

            AddComponent(entity, backGroundScrollData);
        }
    }
}
