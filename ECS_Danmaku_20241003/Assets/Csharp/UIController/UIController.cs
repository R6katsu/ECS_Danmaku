using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;

#if UNITY_EDITOR
#endif

/// <summary>
/// UI選択
/// </summary>
public class UIController : MonoBehaviour
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

    public UnityEvent GetANHFAJON(int2 key)
    {
        UnityEvent unityEvent = null;

        // 最小値を取得
        var xMin = _axisInputEventInfos.Min(info => info.AxisInput.x);
        var yMin = _axisInputEventInfos.Min(info => info.AxisInput.y);

        // 各成分のいずれかの最小値が負の値だった場合、
        // "負の値が含まれているか"というフラグを立てる
        _hasNegativeValue = (xMin < 0 || yMin < 0) ? true : false;

        //  取得可能なイベントの入力方向に負の値が含まれている
        if (_hasNegativeValue)
        {
            // 一つ一つ比較して一致するものを探索する
            foreach (var axisInputEventInfo in _axisInputEventInfos)
            {
                unityEvent = 
                    (axisInputEventInfo.AxisInput.x == key.x &&
                    axisInputEventInfo.AxisInput.y == key.y) ?
                        axisInputEventInfo.AxisInputEvent : null;
            }

            // 一致したものがあればそれを返す、無ければnullを返す
            return unityEvent;
        }
        else
        {
            // 多次元配列を作成して返す、検索した要素が初期値の場合はnullを返す
            return AxisInputEventInfos[key.x, key.y];
        }
    }
}
