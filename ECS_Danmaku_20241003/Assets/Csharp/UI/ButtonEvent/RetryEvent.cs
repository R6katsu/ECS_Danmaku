using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// シーン再読み込み
/// </summary>
public class RetryEvent : MonoBehaviour
{
    [SerializeField, Header("シーン遷移のトランジションの名称")]
    private TransitionName _transitionName = 0;

    private bool _isDisposed = false;

    /// <summary>
    /// シーン再読み込み
    /// </summary>
    public void Retry()
    {
        if (_isDisposed) { return; }

        _isDisposed = true;

        // 現在のシーンのインデックスを取得
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        var transition = TransitionDirector.Instance?.GetTransition(_transitionName);

        if (transition == null)
        {
#if UNITY_EDITOR
            Debug.LogError("transitionの取得に失敗。タイトルから始めてください");
#endif
        }

        transition?.StartTransition(currentSceneIndex);
    }
}
