#if UNITY_EDITOR
#if false
using System.ComponentModel;
#endif
using UnityEditor;
using UnityEngine;
using static DanmakuHelper;
#endif

/// <summary>
/// DanmakuType�ɕύX�����������A�K�v��Component��Setup���s���B
/// �r���h��͋��MonoBehaviour�ɂȂ�B
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public sealed class DanmakuTypeSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private DanmakuType _danmakuType = 0;

    // �O��̒e���̎�ނ�ێ�
    private DanmakuType _previousDanmakuType = 0;

    // Prefab����ݒu�����ۂ̏�����ʉ߂�����
    private bool _isPrefabPlaced = false;

    // �C���X�y�N�^����l���ύX���ꂽ
    private void OnValidate()
    {
        // Prefab����ݒu����
        if (!_isPrefabPlaced)
        {
            _isPrefabPlaced = true;
            _previousDanmakuType = _danmakuType;
            return;
        }

        SetupDanmakuAuthoring(_danmakuType);
    }

    private void OnEnable()
    {
        // �����^�C�����͔�L����
        if (Application.isPlaying)
            enabled = false;
    }

    /// <summary>
    /// DanmakuType�̓��e�ɉ����ĕK�v��Component��Setup���s��
    /// </summary>
    /// <param name="danmakuType">�e���̎��</param>
    private void SetupDanmakuAuthoring(DanmakuType danmakuType)
    {
        // �O�񂩂�ς���Ă��Ȃ���ΕԂ�
        if (_previousDanmakuType == _danmakuType) { return; }

        // �ς���Ă���΍X�V
        _previousDanmakuType = _danmakuType;

        // �����^�C�����͖���
        if (Application.isPlaying) { return; }
        // 
        // Prefab�������ꍇ�͕ύX�����Ԃ�
        if (EditorUtility.IsPersistent(this))
        {
            // Prefab��I�𒆂ɕύX���������ꍇ�̓��b�Z�[�W�o��
            if (Selection.activeGameObject == this.gameObject)
                Debug.LogError("�V�[���ɔz�u���Ă���ݒ肵�Ă�������");

            return;
        }

        // ���g�ɃA�^�b�`����Ă���Component��T��
        // IDanmakuAuthoring���p�����Ă�����j��
        foreach (var component in GetComponents<Component>())
        {
            // IDanmakuAuthoring���p�����Ă��Ȃ��ꍇ�̓R���e�j���[
            if (!(component is IDanmakuAuthoring danmakuAuthoring)) { continue; }

            // �����^�C�����Ȃ�j��Aelse�Ȃ�x����ɑ����j��
            if (Application.isPlaying)
                Destroy(component);
            else
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                        DestroyImmediate(component);
                };
        }

        // DanmakuType�̓��e�ɉ����ĕK�v��Component���A�^�b�`
        switch (danmakuType)
        {
            default:
            case DanmakuType.None:
                break;

            case DanmakuType.N_Way:
                // �����^�C�����Ȃ�ǉ��Aelse�Ȃ�x����ɒǉ�
                if (Application.isPlaying)
                    gameObject.AddComponent<N_Way_DanmakuAuthoring>();
                else
                    EditorApplication.delayCall += () =>
                    {
                        if (this != null)
                            gameObject.AddComponent<N_Way_DanmakuAuthoring>();
                    };
                break;
        }
    }
#endif
}