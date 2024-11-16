using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// アニメーションを再生する。再生が終わるまで待機し、登録された処理を実行する
/// </summary>
public class UIAnimationAndEndAction : SingletonMonoBehaviour<UIAnimationAndEndAction>
{
    [Tooltip("自身のインスタンス")]
    private UIAnimationAndEndAction _instance = null;

    public void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;

            // シーンを跨いで存在する
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// アニメーションを再生する。再生が終わるまで待機し、登録された処理を実行する
    /// </summary>
    /// <param name="animator">再生するAnimator</param>
    /// <param name="triggerName">AnimatorのTrigger変数の名称</param>
    /// <param name="endAction">終了時に実行される処理</param>
    /// <returns>null</returns>
    public IEnumerator AnimationAndEndAction(Animator animator, string triggerName, Action endAction = null)
    {
        // タイトルロゴを再生
        animator.SetTrigger(triggerName);

        // アニメーションの遷移を待つ
        yield return null;

        // アニメーション情報の取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 正しい長さが取得できるまで待機
        while (animator.GetCurrentAnimatorStateInfo(0).length == 1)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        // アニメーション終了まで待機
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // アニメーション終了時に登録された処理を実行
        endAction?.Invoke();
    }
}
