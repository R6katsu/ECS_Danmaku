using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// 有効化可能なUI要素の管理
/// </summary>
public class ActivatableUIDirector : SingletonMonoBehaviour<ActivatableUIDirector>
{
    [SerializeField, Header("有効化可能なUI要素")]
    private RectTransform[] _activatableUIElements = null;

    [Tooltip("メインスレッドで実行する処理を保持するAction")]
    private Queue<Action> _mainThreadAction = null;

    private void OnEnable()
    {
        // 初期化
        _mainThreadAction = new();

        // 全てのUIを無効化
        DisableAllUI();
    }

    private void Update()
    {
        // メインスレッドで順番に処理を実行
        while (_mainThreadAction.Count > 0)
        {
            // Dequeueは処理の後に自動的に削除する
            _mainThreadAction.Dequeue()?.Invoke();
        }
    }

    /// <summary>
    /// メインスレッドで実行する処理を登録
    /// </summary>
    /// <param name="action">登録する処理</param>
    private void Enqueue(Action action)
    {
        // 並行処理で追加するためのlock
        // メインスレッドで実行する処理を登録
        lock (_mainThreadAction)
        {
            _mainThreadAction.Enqueue(action);
        }
    }

    /// <summary>
    /// 単一のUI要素を有効化する。<br/>
    /// 使い方：関数を実行する側でenumを実装、intに変換してください。
    /// </summary>
    /// <param name="elementNumber">有効化するUI要素の番号</param>
    public void ActivateSingleUIElement(int elementNumber)
    {
        // 配列の範囲外だった
        if (elementNumber < 0 || elementNumber >= _activatableUIElements.Length)
        {
#if UNITY_EDITOR
            Debug.LogError($"elementNumber: {elementNumber} が範囲外！");
#endif
            return;
        }

        // 全てのUIを無効化
        DisableAllUI();

        // 対応するUI要素を有効化
        EnableUIElement(elementNumber);
    }

    private void EnableUIElement(int elementNumber)
    {
        // メインスレッドで処理するActionに登録
        Enqueue
        (() =>
        {
            _activatableUIElements[elementNumber].gameObject.SetActive(true);
        }
        );
    }

    /// <summary>
    /// 全てのUIを無効化
    /// </summary>
    public void DisableAllUI()
    {
        // メインスレッドで処理するActionに登録
        Enqueue
        (() =>
        {
            // 全てのUIを無効化
            foreach (var uiElement in _activatableUIElements)
            {
                uiElement.gameObject.SetActive(false);
            }
        }
        );
    }
}
