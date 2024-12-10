using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
#endif

// リファクタリング済み

/// <summary>
/// メインスレッドで処理を実行する
/// </summary>
public class MainThreadExecutor : SingletonMonoBehaviour<MainThreadExecutor>
{
    [Tooltip("メインスレッドで実行する処理を保持するAction")]
    private Queue<Action> _mainThreadAction = new();

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
    public void Enqueue(Action action)
    {
        // 並行処理で追加するためのlock
        // メインスレッドで実行する処理を登録
        lock (_mainThreadAction)
        {
            _mainThreadAction.Enqueue(action);
        }
    }
}
