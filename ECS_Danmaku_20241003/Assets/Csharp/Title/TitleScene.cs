using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// タイトルシーン
/// </summary>
public class TitleScene : MonoBehaviour
{
    private void Update()
    {
        if (!Input.anyKeyDown) { return; }

        // 現在のシーンの番号を取得
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 現在のシーン番号をインクリメント
        currentSceneIndex++;

        // 次のシーンを読み込む
        SceneManager.LoadScene(currentSceneIndex);

        enabled = false;
    }
}
