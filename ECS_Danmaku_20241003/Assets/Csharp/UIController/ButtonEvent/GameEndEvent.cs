using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �Q�[�����I������
/// </summary>
public class GameEndEvent : MonoBehaviour
{
    /// <summary>
    /// �Q�[�����I������
    /// </summary>
    public void GameEnd()
    {
#if UNITY_EDITOR
        // �v���C���[�h���I��
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // �r���h���ꂽ�Q�[�����I��
        Application.Quit();
#endif
    }
}
