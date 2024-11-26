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

        [SerializeField, Header("水平垂直の入力回数と紐つけて呼び出される関数")]
        private UnityEvent _axisInputEvent;

        public int2 AxisInput => _axisInput;
        public UnityEvent AxisInputEvent => _axisInputEvent;
    }

    [SerializeField, Header("水平垂直の入力回数とUnityEventの情報の配列")]
    private AxisInputEventInfo[] _axisInputEventInfos = null;

    private UnityEvent[,] _unityEvents = null;

    [Tooltip("入力方向に負の値が含まれているか")]
    private bool _hasNegativeValue = false;

    [Tooltip("選択中のUIイベント")]
    private int2 _selectedUIEvent = int2.zero;

    [Tooltip("UIの操作が有効か")]
    private bool _isUIEnabled = false;

    // InputSystem
    private PlayerControls _playerInput;

    /// <summary>
    /// 水平垂直の入力回数とUnityEventの情報の多次元配列
    /// </summary>
    private UnityEvent[,] AxisInputEventInfos
    {
        get
        {
            // 多次元配列がnullだった
            if (_unityEvents == null)
            {
                // 多次元配列を作成し、構造体内のint2を多次元配列に置き換える
                var xMax = _axisInputEventInfos.Max(info => info.AxisInput.x);
                var yMax = _axisInputEventInfos.Max(info => info.AxisInput.y);

                _unityEvents = new UnityEvent[xMax, yMax];

                foreach (var info in _axisInputEventInfos)
                {
                    _unityEvents[info.AxisInput.x, info.AxisInput.y] = info.AxisInputEvent;
                }
            }

            return _unityEvents;
        }
    }

    /// <summary>
    /// 操作の有効化
    /// </summary>
    public void Enable()
    {
        // ボタンと選択中のボタンの強調
        // 範囲外に出た時に反対側に戻ってくる処理

        // 最小値を取得
        var xMin = _axisInputEventInfos.Min(info => info.AxisInput.x);
        var yMin = _axisInputEventInfos.Min(info => info.AxisInput.y);

        // 各成分のいずれかの最小値が負の値だった場合、
        // "負の値が含まれているか"というフラグを立てる
        _hasNegativeValue = (xMin < 0 || yMin < 0) ? true : false;

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
                var uiEvent = GetANHFAJON();

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
    public UnityEvent GetANHFAJON()
    {
        // UIの操作が無効だった
        if (!_isUIEnabled) { return null; }

        UnityEvent unityEvent = null;

        //  取得可能なイベントの入力方向に負の値が含まれている
        if (_hasNegativeValue)
        {
            // 一つ一つ比較して一致するものを探索する
            foreach (var axisInputEventInfo in _axisInputEventInfos)
            {
                if (axisInputEventInfo.AxisInput.x == _selectedUIEvent.x &&
                    axisInputEventInfo.AxisInput.y == _selectedUIEvent.y)
                {
                    unityEvent = axisInputEventInfo.AxisInputEvent;
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
    public void SelectedHorizontalIncrement() => _selectedUIEvent.x++;

    /// <summary>
    /// 水平入力をデクリメント
    /// </summary>
    public void SelectedHorizontalDecrement() => _selectedUIEvent.x--;

    /// <summary>
    /// 垂直入力をインクリメント
    /// </summary>
    public void SelectedVerticalIncrement() => _selectedUIEvent.y++;

    /// <summary>
    /// 垂直入力をデクリメント
    /// </summary>
    public void SelectedVerticalDecrement() => _selectedUIEvent.y--;
}
