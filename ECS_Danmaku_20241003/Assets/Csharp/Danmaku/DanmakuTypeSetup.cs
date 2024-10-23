using UnityEditor;
using UnityEngine;
using static DanmakuHelper;
using Component = UnityEngine.Component;

#if UNITY_EDITOR
using System.ComponentModel;
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
    [SerializeField, Header("�e���̎��")]
    private DanmakuType _danmakuType = 0;

    [Tooltip("�O��̒e���̎�ނ�ێ�")]
    private DanmakuType _previousDanmakuType = 0;

    [Tooltip("Prefab����ݒu�����ۂ̏�����ʉ߂������̃t���O")]
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

        // DanmakuType�̓��e�ɉ����ĕK�v��Component��Setup���s��
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
        // �O�񂩂�ς���Ă��Ȃ���ΐ؂�グ��
        if (_previousDanmakuType == _danmakuType) { return; }

        // �����^�C�����͖���
        if (Application.isPlaying) { return; }
        // 
        // Prefab�������ꍇ�͕ύX�����Ԃ�
        if (EditorUtility.IsPersistent(this))
        {
            // Prefab��I�𒆂ɕύX���������ꍇ�̓��b�Z�[�W�o��
            if (Selection.activeGameObject == this.gameObject)
            { Debug.LogError("�V�[���ɔz�u���Ă���ݒ肵�Ă�������"); }

            return;
        }

        // �ς���Ă���΍X�V�𔽉f
        _previousDanmakuType = _danmakuType;

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
            // ��O�Ȃ牽�����Ȃ�
            default:
            case DanmakuType.None:
                break;

            case DanmakuType.N_Way:
                AddComponent<N_Way_DanmakuAuthoring>();
                break;
            case DanmakuType.TapShooting:
                AddComponent<TapShooting_DanmakuAuthoring>();
                break;
        }
    }

    /// <summary>
    /// �����^�C�������ۂ��̔�����܂�AddComponent
    /// </summary>
    /// <typeparam name="T">AddComponent����^</typeparam>
    private void AddComponent<T>() where T : MonoBehaviour
    {
        // �����^�C�����Ȃ�ǉ��Aelse�Ȃ�x����ɒǉ�
        if (Application.isPlaying)
            gameObject.AddComponent<T>();
        else
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                    gameObject.AddComponent<T>();
            };
    }
#endif
}