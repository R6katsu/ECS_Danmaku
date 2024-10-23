using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using UnityEngine;
using static EnemyHelper;
using static HealthPointDatas;
using static PlayerHelper;
using static UnityEngine.EventSystems.EventTrigger;
using static HealthHelper;

#if UNITY_EDITOR
#endif

/// <summary>
/// HealthPointData���Z�܂�������
/// </summary>
public readonly partial struct HealthPointDataAspect : IAspect
{
    // BetterHealthPoint

    public readonly RefRW<PlayerHealthPointData> refRW_PlayerHealthPointData;
    public readonly RefRW<EnemyHealthPointData> refRW_EnemyHealthPointData;

    // Field�ϐ��ɂ��ẮA���̕ϐ�����������Ƃ����C���^�[�t�F�[�X���p������
    // ���̕ϐ����������Ă���Ƃ����C���^�[�t�F�[�X���p�����Ă����ꍇ�A���̕ϐ����g�p�������������֐����Ăׂ�
    // �֐��̎�ނɂ��Ă�enum�Őݒ肷��B�Ȃ�Ȃ�enum�ɂ��Ă��C���^�[�t�F�[�X�Ŏ������A�I���\�Ȋ֐��̂�enum�Ɋ܂߂邩

    // HealthPointDataAspect���Ăяo���Ηl�X��HPData���Ăяo����̂ł͂Ȃ���
    // �Ƃ������A�֐��݂̂�ς��Ă��̑��̏������ꏏ�ɂ�����@���ł���̂ł͂Ȃ���
    // ����قȂ�IHealth���p�������\���̂��쐬����K�v���Ȃ���������Ȃ�

    // PlayerHealthPointData�ł͂Ȃ��ATag�̂悤�Ȃ��̂ɂ���
    // Tag�Ƃ̈Ⴂ�̓t�B�[���h�ϐ������邱��
    // ���̂��߁ATag�Ƃ����\���̖��ł͂Ȃ����̖����K�����l����
    // �֐��̓��e����Tag���h�L�ƕR����

    // �ϐ���Tag���ǂ����ɈقȂ�iEnemy�ɂ͖��G���Ԃ��Ȃ����j�ׁA�ϐ����Ⴄ�ꍇ�͈قȂ�\���̂�p�ӂ���
    // ���G���ԗL�\���̂Ɩ��G���Ԗ��\���̂�p�ӂ��A�X�ɖ��G���ԗL�̒��ł���e���̏�����ς�����悤�ɂ���
    // ��e����񕜎��̏�����enum�������ŕR����Henum�ł����null�񋖗e�Ȃ̂ŏ��������ɐݒ�ł���

    // void Damage() { GetDamage(enum) };

    public enum ASDFGHJKL
    {
        [Tooltip("")] None,
        [Tooltip("")] PlayerDamage
    }

    static public Func<float, Entity, PlayerHealthPointData, PlayerHealthPointData> GetDamage(ASDFGHJKL aSDFGHJKL)
    {
        switch (aSDFGHJKL)
        {
            case ASDFGHJKL.PlayerDamage:
                return PlayerDamage;

            default:
            case ASDFGHJKL.None:
                return null;
        }
    }

    // IHealthPoint�ł͂Ȃ��������Ȃ���΂Ȃ�Ȃ��t�B�[���h�ϐ��̃C���^�[�t�F�[�X
    // ����A���̏ꍇ�͖߂�l��Action�̍Ō�̃C���^�[�t�F�[�X�����ꂼ��قȂ邱�ƂɂȂ��Ă��܂���
    static public PlayerHealthPointData PlayerDamage(float damage, Entity entity, PlayerHealthPointData playerHealthPointData)
    {
        // �Ȃ񂩕�����Ă΂�Ă���H
        // �Ƃ�����HP��9���猸��Ȃ�
        // ��񂪍X�V����Ă��Ȃ����ۂ�
        Debug.Log("PL�̃_���[�W����");

        Debug.Log("�����I�ȃR�[�h�ׁ̈A��Œ���");
        playerHealthPointData.currentHP -= damage;
        playerHealthPointData.lastHitTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;

        Debug.Log($"{damage}�_���[�W���󂯂��B�c��HP{playerHealthPointData.currentHP}");

        // HP�� 0�ȉ��Ȃ�|���
        if (playerHealthPointData.currentHP <= 0)
            playerHealthPointData.Down();

        return playerHealthPointData;
    }
}
