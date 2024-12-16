using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.InputSystem.XR;
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �g�����W�V�����Ŏ��s����֐��ėp
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TransitionHandler : MonoBehaviour
{
    [SerializeField, Header("�L�����\��UI�v�f")]
    private UIController _activatableUIElement = null;

    [SerializeField]
    private AudioClip[] _seClips = null;

    private AudioSource _audioSource = null;

    /// <summary>
    /// ���g��AudioSource���擾
    /// </summary>
    private AudioSource AudioSource
    {
        get
        {
            if (_audioSource == null)
            {
                // RequireComponent
                _audioSource = GetComponent<AudioSource>();
            }

            return _audioSource;
        }
    }

    /// <summary>
    /// UI��L���ɂ���
    /// </summary>
    private void ActivateUI()
    {
        _activatableUIElement?.Enable();
    }

    /// <summary>
    /// SE��PlayOneShot�ōĐ�
    /// </summary>
    /// <param name="seNumber">�Đ�����SE�̗v�f�ԍ�</param>
    private void PlaySE(int seNumber)
    {
        AudioSource.PlayOneShot(_seClips[seNumber]);
    }
}
