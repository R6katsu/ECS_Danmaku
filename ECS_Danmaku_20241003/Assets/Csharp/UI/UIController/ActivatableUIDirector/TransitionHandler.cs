using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.InputSystem.XR;
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// トランジションで実行する関数汎用
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TransitionHandler : MonoBehaviour
{
    [SerializeField, Header("有効化可能なUI要素")]
    private UIController _activatableUIElement = null;

    [SerializeField]
    private AudioClip[] _seClips = null;

    private AudioSource _audioSource = null;

    /// <summary>
    /// 自身のAudioSourceを取得
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
    /// UIを有効にする
    /// </summary>
    private void ActivateUI()
    {
        _activatableUIElement?.Enable();
    }

    /// <summary>
    /// SEをPlayOneShotで再生
    /// </summary>
    /// <param name="seNumber">再生するSEの要素番号</param>
    private void PlaySE(int seNumber)
    {
        AudioSource.PlayOneShot(_seClips[seNumber]);
    }
}
