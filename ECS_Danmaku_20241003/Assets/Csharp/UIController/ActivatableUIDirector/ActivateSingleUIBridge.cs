using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �g�����W�V�����Ŏ��s����֐��ėp
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ActivateSingleUIBridge : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _seClips = null;

    private AudioSource _audioSource = null;

    /// <summary>
    /// ���g��AudioSource���擾�AGet
    /// </summary>
    private AudioSource AudioSource
    {
        get
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            return _audioSource;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ActivateUIBridge()
    {
        // �}�W�b�N�i���o�[�A���Ƃ�(int)enum�ɒu��������
        GameSceneUIDirector.Instance.ActivateSingleUIControllerElement(0);
    }

    /// <summary>
    /// SE��PlayOneShot�ōĐ�
    /// </summary>
    /// <param name="seNumber">�Đ�����SE�̗v�f�ԍ�</param>
    public void PlaySE(int seNumber)
    {
        AudioSource.PlayOneShot(_seClips[seNumber]);
    }
}
