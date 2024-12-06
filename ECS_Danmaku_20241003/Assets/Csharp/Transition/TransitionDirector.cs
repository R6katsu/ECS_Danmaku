using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// �g�����W�V�����̖���
/// </summary>
public enum TransitionName : int
{
    Star
}

/// <summary>
/// �g�����W�V�����̊Ǘ�
/// </summary>
public class TransitionDirector : SingletonMonoBehaviour<TransitionDirector>
{
    private static TransitionDirector _instance = null;

    [SerializeField, Header("�g�����W�V�����z��")]
    private RectTransform[] _transitions = null;

    private void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;

            // �V�[�����ׂ��ő��݂���
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// TransitionName�ɑΉ�����g�����W�V������Ԃ�
    /// </summary>
    /// <returns>TransitionName�ɑΉ�����g�����W�V����</returns>
    public ITransition GetTransition(TransitionName transitionName)
    {
        if (!_transitions[(int)transitionName].TryGetComponent(out ITransition transition))
        {
#if UNITY_EDITOR
            Debug.LogError($"{_transitions[(int)transitionName].gameObject.name}��ITransition��L���Ă��Ȃ�");
#endif
        }

        return transition;
    }
}
