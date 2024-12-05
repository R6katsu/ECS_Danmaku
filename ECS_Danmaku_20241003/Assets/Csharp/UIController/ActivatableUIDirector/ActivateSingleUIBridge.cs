using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSingleUIBridge : MonoBehaviour
{
    [SerializeField, Min(0)]
    private int _playSENumber = 0;

    public void ActivateUIBridge()
    {
        // �}�W�b�N�i���o�[�A���Ƃ�(int)enum�ɒu��������
        GameSceneUIDirector.Instance.ActivateSingleUIControllerElement(0);
    }

    public void PlaySEBridge()
    {
        // BGM�J�n
        AudioPlayManager.Instance.PlaySE(_playSENumber);
    }
}
