using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音源管理
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    private AudioSource _bgmAudioSource = null;

    [SerializeField]
    private AudioSource _seAudioSource = null;

    [SerializeField, Tooltip("BGM")]
    private AudioClip[] _bgmClips = null;

    [SerializeField, Tooltip("効果音")]
    private AudioClip[] _seClips = null;

    /// <summary>
    /// BGM再生
    /// </summary>
    public void PlayBGM(int num)
    {
        if (_bgmAudioSource == null) { return; }

        _bgmAudioSource.clip = _bgmClips[num];
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// 効果音再生
    /// </summary>
    public void PlaySE(int num)
    {
        if (_seAudioSource == null) { return; }

        _seAudioSource.clip = _seClips[num];
        _seAudioSource.Play();
    }
}
