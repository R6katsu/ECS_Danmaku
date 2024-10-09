using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static DanmakuHelper;

// �e����G�ɕK�v��Tag���쐬
// �e���̎�ނɂ���ĕt����Tag���Ⴄ�H
// �e��Tag���������Aenum��ێ������Ă����������B����Ȃ�Tag�̐������点��
// �C���X�y�N�^����enum��I�сAData�����鎞��Tag���A�^�b�`����

public struct Tmp_TestDanmaku_20241008Data : IComponentData
{
    public readonly DanmakuType danmakuType;
    public readonly Entity bulletPrefab;

    public Tmp_TestDanmaku_20241008Data(DanmakuType danmakuType, Entity bulletPrefab)
    {
        this.danmakuType = danmakuType;
        this.bulletPrefab = bulletPrefab;
    }
}

public class Tmp_TestDanmaku_20241008Authoring : MonoBehaviour
{
    // �e�����ƂɊԊu��p�x�Ȃǂ����߂���悤�ɂ���
    // �e�����ƂɁu���͈̔͂��o����폜�v�Ƃ���AABB�����߂���悤�ɂ���

    [SerializeField]
    private DanmakuType _danmakuType = 0;

    [SerializeField]
    private DanmakuTypeSO _danmakuTypeSO = null;

    // �e���Ǝg�p����ePrefab�̐ݒ��SO�ōs���A�����z��ɃC���X�y�N�^����������̂͂ǂ����낤��

    /// <summary>
    /// �e���̎��
    /// </summary>
    private DanmakuType MyDanmakuType => _danmakuType;

    private DanmakuTypeSO DanmakuTypeSO => _danmakuTypeSO;

    public class Baker : Baker<Tmp_TestDanmaku_20241008Authoring>
    {
        public override void Bake(Tmp_TestDanmaku_20241008Authoring src)
        {
            var bulletEntity = GetEntity(src.DanmakuTypeSO.BulletPrefab, TransformUsageFlags.Dynamic);

            var tmp_TestDanmaku_20241008 = new Tmp_TestDanmaku_20241008Data(src.MyDanmakuType, bulletEntity);

            AddComponent(GetEntity(TransformUsageFlags.Dynamic), tmp_TestDanmaku_20241008);
        }
    }
}
