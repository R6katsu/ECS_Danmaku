using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// トランジションの名称
/// </summary>
public enum TransitionName : int
{
    Star
}

/// <summary>
/// トランジションの管理
/// </summary>
public class TransitionDirector : SingletonMonoBehaviour<TransitionDirector>
{
    private static TransitionDirector _instance = null;

    [SerializeField, Header("トランジション配列")]
    private RectTransform[] _transitions = null;

    private void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;

            // シーンを跨いで存在する
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// TransitionNameに対応するトランジションを返す
    /// </summary>
    /// <returns>TransitionNameに対応するトランジション</returns>
    public ITransition GetTransition(TransitionName transitionName)
    {
        if (!_transitions[(int)transitionName].TryGetComponent(out ITransition transition))
        {
#if UNITY_EDITOR
            Debug.LogError($"{_transitions[(int)transitionName].gameObject.name}がITransitionを有していない");
#endif
        }

        return transition;
    }
}
