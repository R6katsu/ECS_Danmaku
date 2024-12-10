using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// ���C���X���b�h�ŏ��������s����
/// </summary>
public class MainThreadExecutor : SingletonMonoBehaviour<MainThreadExecutor>
{
    [Tooltip("���C���X���b�h�Ŏ��s���鏈����ێ�����Action")]
    private Queue<Action> _mainThreadAction = new();

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
    public void Enqueue(Action action)
    {
        // ���s�����Œǉ����邽�߂�lock
        // ���C���X���b�h�Ŏ��s���鏈����o�^
        lock (_mainThreadAction)
        {
            _mainThreadAction.Enqueue(action);
        }
    }
}
