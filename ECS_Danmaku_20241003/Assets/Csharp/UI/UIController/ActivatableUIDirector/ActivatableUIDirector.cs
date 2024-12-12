using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
#endif

// リファクタリング済み

/// <summary>
/// 有効化可能なUI要素の管理
/// </summary>
public class ActivatableUIDirector : SingletonMonoBehaviour<ActivatableUIDirector>
{
    [SerializeField, Header("有効化可能なUI要素")]
    private RectTransform[] _activatableUIElements = null;

    private void OnEnable()
    {
        // 全てのUIを無効化
        DisableAllUI();
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
        MainThreadExecutor.Instance.Enqueue
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
        MainThreadExecutor.Instance.Enqueue
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
