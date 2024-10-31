using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using static VFXCreationBridge;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// �����̎��
/// </summary>
public enum AudioType
{
    BGM,
    SE
}

/// <summary>
/// �����Đ��̏��
/// </summary>
public struct AudioPlayData : IComponentData
{
    private const int LOWER_LIMIT = -1;

    private int audioNumber;
    private bool isPlaying;

    [Tooltip("�����̎��")]
    public readonly AudioType audioType;

    /// <summary>
    /// �Đ����鉹���̔ԍ��B<br/>
    /// ��������ύX���������ꍇ�Ɂu�����Đ��v�̃t���O�𗧂Ă�
    /// </summary>
    public int AudioNumber
    {
        get => audioNumber;
        set
        {
            // ��������ύX���������ꍇ�Ɂu�����\�v�̃t���O�𗧂Ă�
            if (audioNumber != value && value != LOWER_LIMIT)
            {
                audioNumber = value;
                isPlaying = true;
            }
        }
    }

    /// <summary>
    /// �����Đ��\���ǂ����̃t���O
    /// </summary>
    public bool IsPlaying => isPlaying;

    /// <summary>
    /// �����Đ��̏��
    /// </summary>
    public AudioPlayData(AudioType audioType)
    {
        this.audioType = audioType;

        audioNumber = LOWER_LIMIT;
        isPlaying = false;
    }

    /// <summary>
    /// �����Đ��𖳌��ɂ���
    /// </summary>
    public void DisableAudioPlay()
    {
        audioNumber = LOWER_LIMIT;
        isPlaying = false;
    }
}

/// <summary>
/// �����Đ��̐ݒ�
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

            AddComponent(entity, new AudioPlayData(src._audioType));
        }
    }
}
