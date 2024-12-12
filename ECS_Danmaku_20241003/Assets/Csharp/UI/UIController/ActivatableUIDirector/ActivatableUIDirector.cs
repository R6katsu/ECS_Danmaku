using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �L�����\��UI�v�f�̊Ǘ�
/// </summary>
public class ActivatableUIDirector : SingletonMonoBehaviour<ActivatableUIDirector>
{
    [SerializeField, Header("�L�����\��UI�v�f")]
    private RectTransform[] _activatableUIElements = null;

    private void OnEnable()
    {
        // �S�Ă�UI�𖳌���
        DisableAllUI();
    }

    /// <summary>
    /// �P���UI�v�f��L��������B<br/>
    /// �g�����F�֐������s���鑤��enum�������Aint�ɕϊ����Ă��������B
    /// </summary>
    /// <param name="elementNumber">�L��������UI�v�f�̔ԍ�</param>
    public void ActivateSingleUIElement(int elementNumber)
    {
        // �z��͈̔͊O������
        if (elementNumber < 0 || elementNumber >= _activatableUIElements.Length)
        {
#if UNITY_EDITOR
            Debug.LogError($"elementNumber: {elementNumber} ���͈͊O�I");
#endif
            return;
        }

        // �S�Ă�UI�𖳌���
        DisableAllUI();

        // �Ή�����UI�v�f��L����
        EnableUIElement(elementNumber);
    }

    private void EnableUIElement(int elementNumber)
    {
        // ���C���X���b�h�ŏ�������Action�ɓo�^
        MainThreadExecutor.Instance.Enqueue
        (() =>
        {
            _activatableUIElements[elementNumber].gameObject.SetActive(true);
        }
        );
    }

    /// <summary>
    /// �S�Ă�UI�𖳌���
    /// </summary>
    public void DisableAllUI()
    {
        // ���C���X���b�h�ŏ�������Action�ɓo�^
        MainThreadExecutor.Instance.Enqueue
        (() =>
        {
            // �S�Ă�UI�𖳌���
            foreach (var uiElement in _activatableUIElements)
            {
                uiElement.gameObject.SetActive(false);
            }
        }
        );
    }
}
