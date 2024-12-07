using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// トランジションで実行する関数汎用
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ActivateSingleUIBridge : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _seClips = null;

    private AudioSource _audioSource = null;

    /// <summary>
    /// 自身のAudioSourceを取得、Get
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
        // マジックナンバー、あとで(int)enumに置き換える
        GameSceneUIDirector.Instance.ActivateSingleUIControllerElement(0);
    }

    /// <summary>
    /// SEをPlayOneShotで再生
    /// </summary>
    /// <param name="seNumber">再生するSEの要素番号</param>
    public void PlaySE(int seNumber)
    {
        AudioSource.PlayOneShot(_seClips[seNumber]);
    }
}
