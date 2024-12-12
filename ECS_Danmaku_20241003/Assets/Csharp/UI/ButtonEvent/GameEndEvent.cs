using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// ゲームを終了する
/// </summary>
public class GameEndEvent : MonoBehaviour
{
    private bool _isDisposed = false;

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void GameEnd()
    {
        if (_isDisposed) { return; }
        _isDisposed = true;

#if UNITY_EDITOR
        // プレイモードを終了
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ビルドされたゲームを終了
        Application.Quit();
#endif
    }
}
