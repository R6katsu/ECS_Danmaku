using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Collections;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �����̍Đ��Ǘ�
/// </summary>
public class AudioPlayManager : SingletonMonoBehaviour<AudioPlayManager>
{
    [SerializeField]
    private AudioSource _bgmAudioSource = null;

    [SerializeField]
    private AudioSource _seAudioSource = null;

    [SerializeField, Header("BGM�z��")]
    private AudioClip[] _bgmClips = null;

    [SerializeField, Header("SE�z��")]
    private AudioClip[] _seClips = null;

    /// <summary>
    /// BGM�Đ�
    /// </summary>
    /// <param name="num">�����ԍ�</param>
    public void PlayBGM(int num)
    {
        // AudioSource��null�A�܂��͈������z��̗v�f���𒴉߂��Ă���
        if (_bgmAudioSource == null
            || _bgmClips.Length <= num) { return; }

        // �����Đ�
        _bgmAudioSource.clip = _bgmClips[num];
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// BGM�Đ�
    /// </summary>
    /// <param name="num">�����ԍ�</param>
    public void PauseBGM()
    {
        // AudioSource��null������
        if (_bgmAudioSource == null) { return; }

        // �����ꎞ��~
        _bgmAudioSource.Pause();
    }

    /// <summary>
    /// SE�Đ�
    /// </summary>
    /// <param name="num">�����ԍ�</param>
    public void PlaySE(int num)
    {
        // AudioSource��null�A�܂��͈������z��̗v�f���𒴉߂��Ă���
        if (_seAudioSource == null 
            || _seClips.Length <= num) { return; }

        // PlayOneShot�ŉ����Đ�
        _seAudioSource.PlayOneShot(_seClips[num]);
    }
}
