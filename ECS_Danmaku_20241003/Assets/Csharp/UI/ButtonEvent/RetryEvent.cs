using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �V�[���ēǂݍ���
/// </summary>
public class RetryEvent : MonoBehaviour
{
    [SerializeField, Header("�V�[���J�ڂ̃g�����W�V�����̖���")]
    private TransitionName _transitionName = 0;

    /// <summary>
    /// �V�[���ēǂݍ���
    /// </summary>
    public void Retry()
    {
        // ���݂̃V�[���̃C���f�b�N�X���擾
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        var transition = TransitionDirector.Instance?.GetTransition(_transitionName);

        if (transition == null)
        {
#if UNITY_EDITOR
            Debug.LogError("transition�̎擾�Ɏ��s�B�^�C�g������n�߂Ă�������");
#endif
        }

        transition.StartTransition(currentSceneIndex);
    }
}
