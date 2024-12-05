using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSingleUIBridge : MonoBehaviour
{
    [SerializeField, Min(0)]
    private int _playSENumber = 0;

    public void ActivateUIBridge()
    {
        // マジックナンバー、あとで(int)enumに置き換える
        GameSceneUIDirector.Instance.ActivateSingleUIControllerElement(0);
    }

    public void PlaySEBridge()
    {
        // BGM開始
        AudioPlayManager.Instance.PlaySE(_playSENumber);
    }
}
