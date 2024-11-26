using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
#endif

/// <summary>
/// 
/// </summary>
public class GameSceneUIDirector : SingletonMonoBehaviour<GameSceneUIDirector>
{
    [SerializeField, Header("UI操作の配列")]
    private UIController[] _activatableUIControllerElements = null;

    public void Retry()
    {
        // 現在のシーンのインデックスを取得
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 現在のシーンを読み込む
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void GameEnd()
    {
#if UNITY_EDITOR
        // プレイモードを終了
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ビルドされたゲームを終了
            Application.Quit();
#endif
    }

    /// <summary>
    /// 単一のUI要素を有効化する。<br/>
    /// 使い方：関数を実行する側でenumを実装、intに変換してください。
    /// </summary>
    /// <param name="elementNumber">有効化するUI要素の番号</param>
    public void ActivateSingleUIControllerElement(int elementNumber)
    {
        // 配列の範囲外だった
        if (elementNumber < 0 || elementNumber >= _activatableUIControllerElements.Length)
        {
#if UNITY_EDITOR
            Debug.LogError($"elementNumber: {elementNumber} が範囲外！");
#endif
            return;
        }

        var uiController = _activatableUIControllerElements[elementNumber];

        uiController.Enable();
    }
}
