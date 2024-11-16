using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �A�j���[�V�������Đ�����B�Đ����I���܂őҋ@���A�o�^���ꂽ���������s����
/// </summary>
public class UIAnimationAndEndAction : SingletonMonoBehaviour<UIAnimationAndEndAction>
{
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
    /// <returns>null</returns>
    public IEnumerator AnimationAndEndAction(Animator animator, string triggerName, Action endAction = null)
    {
        // �^�C�g�����S���Đ�
        animator.SetTrigger(triggerName);

        // �A�j���[�V�����̑J�ڂ�҂�
        yield return null;

        // �A�j���[�V�������̎擾
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // �������������擾�ł���܂őҋ@
        while (animator.GetCurrentAnimatorStateInfo(0).length == 1)
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
