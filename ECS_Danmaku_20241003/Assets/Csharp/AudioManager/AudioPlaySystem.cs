using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �����Đ�
/// </summary>
public partial struct AudioPlaySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // ���C���X���b�h�Ŋ֐����Ăяo���ׂ�System
        foreach (var audioPlay in
                 SystemAPI.Query<RefRW<AudioPlayData>>())
        {
            // �Đ��\�t���O�������Ă��Ȃ���΃R���e�j���[
            if (!audioPlay.ValueRO.IsPlaying) { continue; }

            if (audioPlay.ValueRO.audioType == AudioType.BGM)
            {
                AudioManager.Instance.PlayBGM(audioPlay.ValueRO.AudioNumber);
            }
            else
            {
                AudioManager.Instance.PlaySE(audioPlay.ValueRO.AudioNumber);
            }

            // �Đ��𖳌��ɂ���
            audioPlay.ValueRW.DisableAudioPlay();
        }
    }
}
