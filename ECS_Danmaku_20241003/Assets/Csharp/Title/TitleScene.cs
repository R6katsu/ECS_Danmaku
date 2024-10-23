using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

/// <summary>
/// �^�C�g���V�[��
/// </summary>
public class TitleScene : MonoBehaviour
{
    private void Update()
    {
        if (!Input.anyKeyDown) { return; }

        // ���݂̃V�[���̔ԍ����擾
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���݂̃V�[���ԍ����C���N�������g
        currentSceneIndex++;

        // ���̃V�[����ǂݍ���
        SceneManager.LoadScene(currentSceneIndex);

        enabled = false;
    }
}
