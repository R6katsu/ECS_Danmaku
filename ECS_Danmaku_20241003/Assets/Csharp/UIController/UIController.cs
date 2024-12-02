using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
#endif

/// <summary>
/// UI選択
/// </summary>
public class UIController : MonoBehaviour, IDisposable
{
    /// <summary>
    /// 水平垂直の入力回数とUnityEventの情報
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

        public int2 AxisInput => _axisInput;
        public RectTransform ButtonImage => _buttonImage;
        public UnityEvent AxisInputEvent => _axisInputEvent;
        public (RectTransform, UnityEvent) ButtonImage_AxisInputEvent => (_buttonImage, _axisInputEvent);
    }

    [SerializeField, Header("水平垂直の入力回数とUnityEventの情報の配列")]
    private AxisInputEventInfo[] _axisInputEventInfos = null;

    private (RectTransform, UnityEvent)[,] _unityEvents = null;

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
    /// 水平垂直の入力回数とUnityEventの情報の多次元配列
    /// </summary>
    private (RectTransform, UnityEvent)[,] AxisInputEventInfos
    {
        get
        {
            // 多次元配列がnullだった
            if (_unityEvents == null)
            {
                // 多次元配列を作成し、構造体内のint2を多次元配列に置き換える
                _unityEvents = new (RectTransform, UnityEvent)[_xValueMax, _yValueMax];

                foreach (var info in _axisInputEventInfos)
                {
                    _unityEvents[info.AxisInput.x, info.AxisInput.y] = info.ButtonImage_AxisInputEvent;
                }
            }

            return _unityEvents;
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
                UnityEvent uiEvent = GetANHFAJON().Value.Item2;

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
                float inputValue = context.ReadValue<float>();

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
                float inputValue = context.ReadValue<float>();

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
    /// 
    /// </summary>
    /// <returns></returns>
    public (RectTransform, UnityEvent)? GetANHFAJON()
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
            return AxisInputEventInfos[_selectedUIEvent.x, _selectedUIEvent.y];
        }
    }

    /// <summary>
    /// 水平入力をインクリメント
    /// </summary>
    public void SelectedHorizontalIncrement()
    {
        AAAAA();

        _selectedUIEvent.x++;

        // x方向の最大値を超過した場合は最小値を代入
        _selectedUIEvent.x = (_selectedUIEvent.x > _xValueMax) ? _xValueMin : _selectedUIEvent.x;

        BBBBB();
    }

    /// <summary>
    /// 水平入力をデクリメント
    /// </summary>
    public void SelectedHorizontalDecrement()
    {
        AAAAA();

        _selectedUIEvent.x--;

        // x方向の最小値未満だった場合は最大値を代入
        _selectedUIEvent.x = (_selectedUIEvent.x < _xValueMin) ? _xValueMax : _selectedUIEvent.x;

        BBBBB();
    }

    /// <summary>
    /// 垂直入力をインクリメント
    /// </summary>
    public void SelectedVerticalIncrement()
    {
        AAAAA();

        _selectedUIEvent.y++;

        // y方向の最大値を超過した場合は最小値を代入
        _selectedUIEvent.y = (_selectedUIEvent.y > _yValueMax) ? _yValueMin : _selectedUIEvent.y;

        BBBBB();
    }

    /// <summary>
    /// 垂直入力をデクリメント
    /// </summary>
    public void SelectedVerticalDecrement()
    {
        AAAAA();

        _selectedUIEvent.y--;

        // y方向の最小値未満だった場合は最大値を代入
        _selectedUIEvent.y = (_selectedUIEvent.y < _yValueMin) ? _yValueMax : _selectedUIEvent.y;

        BBBBB();
    }

    private void AAAAA()
    {
        RectTransform buttonImage = GetANHFAJON().Value.Item1;
        buttonImage.localScale = Vector2.one;
    }

    private void BBBBB()
    {
        // 前回選択していた画像を求めて大きさを元に戻す
        // 今回選択中のボタンを大きくする
        RectTransform buttonImage = GetANHFAJON().Value.Item1;
        buttonImage.localScale = Vector2.one * 1.5f;
    }
}
