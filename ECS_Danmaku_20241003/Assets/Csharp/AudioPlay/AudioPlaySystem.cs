using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Transforms;
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �����̍Đ������B<br/>
/// �����̍Đ������C���X���b�h�ōs�����߂�System
/// </summary>
public partial struct AudioPlaySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // �����̍Đ�����
        foreach (var audioPlay in
                 SystemAPI.Query<RefRW<AudioPlayData>>())
        {
            // �Đ��\�t���O�������Ă��Ȃ���΃R���e�j���[
            if (!audioPlay.ValueRO.IsPlaying) { continue; }

            // �����Đ��������Ăяo��
            CallAudioPlay(audioPlay.ValueRO.audioType, audioPlay);
        }
    }

    /// <summary>
    /// �����Đ��������Ăяo��
    /// </summary>
    /// <param name="audioType">�����̎��</param>
    /// <param name="audioPlay">�����Đ��ɕK�v�ȏ��</param>
    private void CallAudioPlay(AudioType audioType, RefRW<AudioPlayData> audioPlay)
    {
        switch (audioType)
        {
            case AudioType.BGM:
                AudioPlayManager.Instance.PlayBGM(audioPlay.ValueRW.AudioNumber);
                break;

            case AudioType.SE:
                AudioPlayManager.Instance.PlaySE(audioPlay.ValueRW.AudioNumber);
                break;

            case AudioType.None:
            default:
#if UNITY_EDITOR
                Debug.LogError("audioType�����ݒ�A�܂��͗�O");
#endif
                break;
        }
    }
}
