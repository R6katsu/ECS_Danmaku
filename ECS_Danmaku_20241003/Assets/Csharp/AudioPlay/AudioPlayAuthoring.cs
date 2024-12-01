using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using static VFXCreationBridge;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �����Đ��ɕK�v�ȏ��
/// </summary>
public struct AudioPlayData : IComponentData
{
    private const int LOWER_LIMIT = -1;

    private int _audioNumber;
    private bool _isPlaying;

    [Tooltip("�����̎��")]
    public readonly AudioType audioType;

    /// <summary>
    /// �Đ����鉹���̔ԍ��B<br/>
    /// ��������ύX���������ꍇ�Ɂu�����Đ��v�̃t���O�𗧂Ă�
    /// </summary>
    public int AudioNumber
    {
        get
        {
            // �������O�̉����ԍ���ێ�
            var audioNumber = _audioNumber;

            // �����Đ��𖳌��ɂ���
            DisableAudioPlay();

            return audioNumber;
        }
        set
        {
            // ��������ύX���������ꍇ�Ɂu�����\�v�̃t���O�𗧂Ă�
            if (_audioNumber != value && value != LOWER_LIMIT)
            {
                _audioNumber = value;
                _isPlaying = true;
            }
        }
    }

    /// <summary>
    /// �����Đ��\���ǂ����̃t���O
    /// </summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// �����Đ��̏��
    /// </summary>
    /// <param name="audioType">�����̎��</param>
    public AudioPlayData(AudioType audioType)
    {
        this.audioType = audioType;

        _audioNumber = LOWER_LIMIT;
        _isPlaying = false;
    }

    /// <summary>
    /// �����Đ��𖳌��ɂ���
    /// </summary>
    public void DisableAudioPlay()
    {
        _audioNumber = LOWER_LIMIT;
        _isPlaying = false;
    }
}

/// <summary>
/// �����Đ��ɕK�v�Ȑݒ�
/// </summary>
public class AudioPlayAuthoring : MonoBehaviour
{
    [SerializeField, Header("�����̎��")]
    private AudioType _audioType = 0;

    public class Baker : Baker<AudioPlayAuthoring>
    {
        public override void Bake(AudioPlayAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // ���g��Entity��AudioPlayData���A�^�b�`
            AddComponent(entity, new AudioPlayData(src._audioType));
        }
    }
}
