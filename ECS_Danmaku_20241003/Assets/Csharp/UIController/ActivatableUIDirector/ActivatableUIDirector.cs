using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// �L�����\��UI�v�f�̊Ǘ�
/// </summary>
public class ActivatableUIDirector : SingletonMonoBehaviour<ActivatableUIDirector>
{
    [SerializeField, Header("�L�����\��UI�v�f")]
    private RectTransform[] _activatableUIElements = null;

    [Tooltip("���C���X���b�h�Ŏ��s���鏈����ێ�����Action")]
    private Queue<Action> _mainThreadAction = null;

    private void OnEnable()
    {
        // ������
        _mainThreadAction = new();

        // �S�Ă�UI�𖳌���
        DisableAllUI();
    }

    private void Update()
    {
        // ���C���X���b�h�ŏ��Ԃɏ��������s
        while (_mainThreadAction.Count > 0)
        {
            // Dequeue�͏����̌�Ɏ����I�ɍ폜����
            _mainThreadAction.Dequeue()?.Invoke();
        }
    }

    /// <summary>
    /// ���C���X���b�h�Ŏ��s���鏈����o�^
    /// </summary>
    /// <param name="action">�o�^���鏈��</param>
    private void Enqueue(Action action)
    {
        // ���s�����Œǉ����邽�߂�lock
        // ���C���X���b�h�Ŏ��s���鏈����o�^
        lock (_mainThreadAction)
        {
            _mainThreadAction.Enqueue(action);
        }
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
        Enqueue
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
        Enqueue
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
