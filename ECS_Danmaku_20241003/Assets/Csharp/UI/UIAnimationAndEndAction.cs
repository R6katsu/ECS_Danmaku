using System;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �A�j���[�V�������Đ�����B�Đ����I���܂őҋ@���A�o�^���ꂽ���������s����
/// </summary>
public class UIAnimationAndEndAction : SingletonMonoBehaviour<UIAnimationAndEndAction>
{
    [Tooltip("�A�j���[�V�����̒����̍ő�l")]
    const float DEFAULT_ANIMATION_LENGTH = 1.0f;

    [Tooltip("���g�̃C���X�^���X")]
    private UIAnimationAndEndAction _instance = null;

    public void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;

            // �V�[�����ׂ��ő��݂���
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// �A�j���[�V�������Đ�����B�Đ����I���܂őҋ@���A�o�^���ꂽ���������s����
    /// </summary>
    /// <param name="animator">�Đ�����Animator</param>
    /// <param name="triggerName">Animator��Trigger�ϐ��̖���</param>
    /// <param name="endAction">�I�����Ɏ��s����鏈��</param>
    /// <returns></returns>
    public IEnumerator AnimationAndEndAction(Animator animator, string triggerName, Action endAction = null)
    {
        // �^�C�g�����S���Đ�
        animator.SetTrigger(triggerName);

        // �A�j���[�V�����̑J�ڂ�҂�
        yield return null;

        // �A�j���[�V�������̎擾
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // �������������擾�ł���܂őҋ@
        while (animator.GetCurrentAnimatorStateInfo(0).length == DEFAULT_ANIMATION_LENGTH)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        // �A�j���[�V�����I���܂őҋ@
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // �A�j���[�V�����I�����ɓo�^���ꂽ���������s
        endAction?.Invoke();
    }
}
