using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.InputSystem;
#endif

// リファクタリング済み

/// <summary>
/// 水平、垂直の入力回数とUnityEventの情報
/// </summary>
[Serializable]
public struct AxisInputEventInfo
{
    [SerializeField, Header("水平垂直の入力回数")]
    private int2 _axisInput;

    [SerializeField, Header("ボタン画像")]
    private RectTransform _buttonImage;

    [SerializeField, Header("水平垂直の入力回数と紐つけて呼び出される関数")]
    private UnityEvent _axisInputEvent;

    /// <summary>
    /// 水平垂直の入力回数
    /// </summary>
    public int2 AxisInput => _axisInput;

    /// <summary>
    /// ボタン画像
    /// </summary>
    public RectTransform ButtonImage => _buttonImage;

    /// <summary>
    /// 水平垂直の入力回数と紐つけて呼び出される関数
    /// </summary>
    public UnityEvent AxisInputEvent => _axisInputEvent;

    /// <summary>
    /// UIと関数のタプル
    /// </summary>
    public (RectTransform, UnityEvent) ButtonImage_AxisInputEvent => (_buttonImage, _axisInputEvent);
}

/// <summary>
/// UI選択
/// </summary>
public class UIController : MonoBehaviour, IDisposable
{
    [Tooltip("デフォルトのUIの大きさ")]
    private readonly Vector2 _defaultUIScale = Vector2.one;

    [SerializeField, Header("水平垂直の入力回数とUnityEventの構造体の配列")]
    private AxisInputEventInfo[] _axisInputEventInfos = null;

    [SerializeField, Min(0.0f), Header("選択中のUIの大きさ")]
    private float _selectedUIScale = 0.0f;

    [Tooltip("UIとUnityEventを紐つけた多次元配列")]
    private (RectTransform, UnityEvent)[,] _uiAndUnityEvents = null;

    [Tooltip("入力方向に負の値が含まれているか")]
    private bool _hasNegativeValue = false;

    [Tooltip("選択中のUIイベント")]
    private int2 _selectedUIEvent = int2.zero;

    [Tooltip("UIの操作が有効か")]
    private bool _isUIEnabled = false;

    // InputSystem
    private PlayerControls _playerInput;

    // 各成分の最大値、最小値
    private int _xValueMax = 0;
    private int _yValueMax = 0;
    private int _xValueMin = 0;
    private int _yValueMin = 0;

    /// <summary>
    /// UIとUnityEventを紐つけた多次元配列
    /// </summary>
    private (RectTransform, UnityEvent)[,] UIAndUnityEvents
    {
        get
        {
            // 多次元配列がnullだった
            if (_uiAndUnityEvents == null)
            {
                // 多次元配列を作成し、構造体内のint2を多次元配列に置き換える
                _uiAndUnityEvents = new (RectTransform, UnityEvent)[_xValueMax, _yValueMax];

                foreach (var info in _axisInputEventInfos)
                {
                    _uiAndUnityEvents[info.AxisInput.x, info.AxisInput.y] = info.ButtonImage_AxisInputEvent;
                }
            }

            return _uiAndUnityEvents;
        }
    }

    private void OnEnable()
    {
        // 既に有効だった
        if (_isUIEnabled) { return; }

        // ボタン画像を無効化
        foreach (var info in _axisInputEventInfos)
        {
            info.ButtonImage_AxisInputEvent.Item1.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 操作の有効化
    /// </summary>
    public void Enable()
    {
        // 最大値を取得
        _xValueMax = _axisInputEventInfos.Max(info => info.AxisInput.x);
        _yValueMax = _axisInputEventInfos.Max(info => info.AxisInput.y);

        // 最小値を取得
        _xValueMin = _axisInputEventInfos.Min(info => info.AxisInput.x);
        _yValueMin = _axisInputEventInfos.Min(info => info.AxisInput.y);

        // 各成分のいずれかの最小値が負の値だった場合、
        // "負の値が含まれているか"というフラグを立てる
        _hasNegativeValue = (_xValueMin < 0 || _yValueMin < 0) ? true : false;

        // メインスレッドで処理するActionに登録
        MainThreadExecutor.Instance.Enqueue
        (() =>
        {
            // 初期化
            _selectedUIEvent = int2.zero;

            _playerInput = new PlayerControls();
            _playerInput.Enable();

            // Shotに割り当てる
            var confirm = _playerInput.UI.Confirm;
            confirm.started += (context) =>
            {
                UnityEvent uiEvent = GetUIAndUnityEvent().Value.Item2;

                // メインスレッドで処理するActionに登録
                MainThreadExecutor.Instance.Enqueue
                (() =>
                {
                    uiEvent?.Invoke();
                }
                );
            };

            // Horizontalに割り当てる
            var horizontal = _playerInput.UI.Horizontal;
            horizontal.started += (context) =>
            {
                var inputValue = context.ReadValue<float>();

                if (inputValue > 0)
                { // 右方向の入力
                    SelectedHorizontalIncrement();
                }
                else if (inputValue < 0)
                { // 左方向の入力
                    SelectedHorizontalDecrement();
                }
            };

            // Verticalに割り当てる
            var vertical = _playerInput.UI.Vertical;
            vertical.started += (context) =>
            {
                var inputValue = context.ReadValue<float>();

                if (inputValue > 0)
                { // 上方向の入力
                    SelectedVerticalIncrement();
                }
                else if (inputValue < 0)
                { // 下方向の入力
                    SelectedVerticalDecrement();
                }
            };

            // ボタン画像を有効化
            foreach (var info in _axisInputEventInfos)
            {
                info.ButtonImage_AxisInputEvent.Item1.gameObject.SetActive(true);
            }

            // 操作の有効化
            _isUIEnabled = true;
        }
        );
    }

    // IDisposable
    public void Dispose()
    {
        if (_playerInput != null)
        {
            // 操作の無効化、リソース解放
            _playerInput.Disable();
            _playerInput.Dispose();
        }

        // 操作の無効化
        _isUIEnabled = false;
    }

    public void OnDisable()
    {
        Dispose();
    }

    /// <summary>
    /// 現在選択中の位置に紐つけられたUIとUnityEventを返す
    /// </summary>
    /// <returns>現在選択中の位置に紐つけられたUIとUnityEvent</returns>
    public (RectTransform, UnityEvent)? GetUIAndUnityEvent()
    {
        // UIの操作が無効だった
        if (!_isUIEnabled) { return null; }

        (RectTransform, UnityEvent) unityEvent = new();

        //  取得可能なイベントの入力方向に負の値が含まれている
        if (_hasNegativeValue)
        {
            // 一つ一つ比較して一致するものを探索する
            foreach (var axisInputEventInfo in _axisInputEventInfos)
            {
                if (axisInputEventInfo.AxisInput.x == _selectedUIEvent.x &&
                    axisInputEventInfo.AxisInput.y == _selectedUIEvent.y)
                {
                    unityEvent = axisInputEventInfo.ButtonImage_AxisInputEvent;
                }
            }

            // 一致したものがあればそれを返す、無ければnullを返す
            return unityEvent;
        }
        else
        {
            // 多次元配列を作成して返す、検索した要素が初期値の場合はnullを返す
            return UIAndUnityEvents[_selectedUIEvent.x, _selectedUIEvent.y];
        }
    }

    /// <summary>
    /// 水平入力をインクリメント
    /// </summary>
    public void SelectedHorizontalIncrement()
    {
        SetDefaultUIScale();

        _selectedUIEvent.x++;

        // x方向の最大値を超過した場合は最小値を代入
        _selectedUIEvent.x = (_selectedUIEvent.x > _xValueMax) ? _xValueMin : _selectedUIEvent.x;

        SetSelectedUIScale();
    }

    /// <summary>
    /// 水平入力をデクリメント
    /// </summary>
    public void SelectedHorizontalDecrement()
    {
        SetDefaultUIScale();

        _selectedUIEvent.x--;

        // x方向の最小値未満だった場合は最大値を代入
        _selectedUIEvent.x = (_selectedUIEvent.x < _xValueMin) ? _xValueMax : _selectedUIEvent.x;

        SetSelectedUIScale();
    }

    /// <summary>
    /// 垂直入力をインクリメント
    /// </summary>
    public void SelectedVerticalIncrement()
    {
        SetDefaultUIScale();

        _selectedUIEvent.y++;

        // y方向の最大値を超過した場合は最小値を代入
        _selectedUIEvent.y = (_selectedUIEvent.y > _yValueMax) ? _yValueMin : _selectedUIEvent.y;

        SetSelectedUIScale();
    }

    /// <summary>
    /// 垂直入力をデクリメント
    /// </summary>
    public void SelectedVerticalDecrement()
    {
        SetDefaultUIScale();

        _selectedUIEvent.y--;

        // y方向の最小値未満だった場合は最大値を代入
        _selectedUIEvent.y = (_selectedUIEvent.y < _yValueMin) ? _yValueMax : _selectedUIEvent.y;

        SetSelectedUIScale();
    }

    /// <summary>
    /// UIの大きさをデフォルトの値に設定する
    /// </summary>
    private void SetDefaultUIScale()
    {
        var uiTransform = GetUIAndUnityEvent().Value.Item1;
        uiTransform.localScale = _defaultUIScale;
    }

    /// <summary>
    /// UIの大きさを選択時の拡大値に設定する
    /// </summary>
    private void SetSelectedUIScale()
    {
        var uiTransform = GetUIAndUnityEvent().Value.Item1;
        uiTransform.localScale = _defaultUIScale * _selectedUIScale;
    }
}
