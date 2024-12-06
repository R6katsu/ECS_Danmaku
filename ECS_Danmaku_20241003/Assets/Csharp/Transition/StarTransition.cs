using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.Collections.Generic;
#endif

/// <summary>
/// 星型のトランジション
/// </summary>
[RequireComponent(typeof(Animator))]
public class StarTransition : MonoBehaviour, ITransition
{
    [Tooltip("自身のインスタンス")]
    private StarTransition _instance = null;

    [Tooltip("シーン遷移のAnimator")]
    private Animator _transitionAnimator = null;

    [SerializeField, Header("シーン遷移開始の再生Trigger")]
    private string _transitionStartTriggerName = null;

    [SerializeField, Header("シーン遷移終了の再生Trigger")]
    private string _transitionEndTriggerName = null;

    /// <summary>
    /// シーン遷移のAnimator
    /// </summary>
    private Animator TransitionAnimator
    {
        get
        {
            // nullだった
            if (_transitionAnimator == null)
            {
                // RequireComponent
                _transitionAnimator = GetComponent<Animator>();
            }

            return _transitionAnimator;
        }
    }

    /// <summary>
    /// トランジションの開始
    /// </summary>
    public void StartTransition(int sceneNumber)
    {
        if (_instance == null)
        {
            _instance = this;

            // シーンを跨いで存在する
            DontDestroyOnLoad(transform.parent.gameObject);
            DontDestroyOnLoad(gameObject);
        }

        StartCoroutine(Transition(sceneNumber));
    }

    /// <summary>
    /// トランジション
    /// </summary>
    public IEnumerator Transition(int sceneNumber)
    {
        // トランジションを開始
        TransitionAnimator.SetTrigger(_transitionStartTriggerName);

        // アニメーションの遷移を待つ
        yield return null;

        // アニメーション情報の取得
        AnimatorStateInfo stateInfo = TransitionAnimator.GetCurrentAnimatorStateInfo(0);

        // 正しい長さが取得できるまで待機
        while (stateInfo.length == 1)
        {
            yield return null;
            stateInfo = TransitionAnimator.GetCurrentAnimatorStateInfo(0);
        }

        // アニメーション終了まで待機
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // シーン遷移
        yield return SceneManager.LoadSceneAsync(sceneNumber);

        // トランジションを終了
        TransitionAnimator.SetTrigger(_transitionEndTriggerName);

        // アニメーションの遷移を待つ
        yield return null;

        // アニメーション情報の取得
        stateInfo = TransitionAnimator.GetCurrentAnimatorStateInfo(0);

        // 正しい長さが取得できるまで待機
        while (stateInfo.length == 1)
        {
            yield return null;
            stateInfo = TransitionAnimator.GetCurrentAnimatorStateInfo(0);
        }

        // アニメーション終了まで待機
        animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);
    }
}
