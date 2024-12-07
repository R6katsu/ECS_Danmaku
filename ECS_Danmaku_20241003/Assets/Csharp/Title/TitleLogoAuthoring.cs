using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
#endif

/// <summary>
/// �^�C�g�����S�̏��i�V���O���g���O��j
/// </summary>
public struct TitleLogoSingletonData : IComponentData
{
    [Tooltip("����̔�e�ŉ摜��؂�ւ��邩")]
    public readonly int imageSwapCount;

    [Tooltip("�^�C�g�����S�̃_���[�W���ʉ�")]
    public readonly int damageSENumber;

    [Tooltip("���݂̔�e��")]
    public int currentImageSwapCount;

    [Tooltip("���̉摜�ɐ؂�ւ��邩")]
    public bool isNextImage;

    /// <summary>
    /// �^�C�g�����S�̃_���[�W���ʉ�
    /// </summary>
    /// <param name="imageSwapCount">����ŉ摜��؂�ւ��邩</param>
    /// <param name="damageSENumber">�^�C�g�����S�̃_���[�W���ʉ�</param>
    public TitleLogoSingletonData(int imageSwapCount, int damageSENumber)
    {
        this.imageSwapCount = imageSwapCount;
        this.damageSENumber = damageSENumber;
        currentImageSwapCount = this.imageSwapCount;
        isNextImage = false;
    }

    /// <summary>
    /// ���̉摜�ɐ؂�ւ���
    /// </summary>
    public void NextImage()
    {
        // ���̉摜�ɐ؂�ւ���t���O�������Ă��Ȃ�
        if (!isNextImage) { return; }

        // �摜��؂�ւ���
        var isTitleLogoBreak = TitleLogoSingleton.Instance.NextImage();

        // �^�C�g�����S�̔j�󂪊�������
        if (isTitleLogoBreak != null && !(bool)isTitleLogoBreak)
        {
            // �j�󂪊���������̏������o�^����Ă�������s����
            TitleLogoSingleton.Instance.breakAction?.Invoke();
        }

        isNextImage = false;
    }
}

/// <summary>
/// �^�C�g�����S�̐ݒ�
/// </summary>
public class TitleLogoAuthoring : MonoBehaviour
{
    [SerializeField, Min(0), Header("����ŉ摜��؂�ւ��邩")]
    private int _imageSwapCount = 0;

    [SerializeField, Min(0), Header("�^�C�g�����S�̃_���[�W���ʉ�")]
    private int _damageSENumber = 0;

    public class Baker : Baker<TitleLogoAuthoring>
    {
        public override void Bake(TitleLogoAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var titleLogoSingletonData = new TitleLogoSingletonData
                (
                    src._imageSwapCount,
                    src._damageSENumber
                );

            AddComponent(entity, titleLogoSingletonData);
        }
    }
}
