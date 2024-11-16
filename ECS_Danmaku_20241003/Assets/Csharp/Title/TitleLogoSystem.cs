using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static BulletHelper;
using static EntityCampsHelper;
using static HealthPointDatas;
using static TriggerJobs;
using EventTrigger = UnityEngine.EventSystems.EventTrigger;

#if UNITY_EDITOR
#endif

/// <summary>
/// �^�C�g�����S�̏���
/// </summary>
//[BurstCompile]
public partial struct TitleLogoSystem : ISystem
{
    private ComponentLookup<TitleLogoSingletonData> _titleLogoSingletonLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;

    public void OnCreate(ref SystemState state)
    {
        // �擾����
        _titleLogoSingletonLookup = state.GetComponentLookup<TitleLogoSingletonData>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        // �X�V����
        _titleLogoSingletonLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);

        // �^�C�g�����S�ɒe�������������̏������Ăяo��
        var titleLogo = new TitleLogoTriggerJob()
        {
            titleLogoSingletonLookup = _titleLogoSingletonLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup
        };

        // TitleLogoTriggerJob���X�P�W���[��
        var titleLogoJobHandle = titleLogo.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = titleLogoJobHandle;

        // �W���u������ҋ@���Ă��玟�̏��������s����
        titleLogoJobHandle.Complete();

        foreach (var (titleLogoSingleton, entity) in SystemAPI.Query<TitleLogoSingletonData>().WithEntityAccess())
        {
            // ���̉摜�ɐ؂�ւ���
            titleLogoSingleton.NextImage();

            // isNextImage �̒l�����ۂ̃G���e�B�e�B�ɔ��f
            state.EntityManager.SetComponentData(entity, titleLogoSingleton);
        }
    }
}

/// <summary>
/// 
/// </summary>
//[BurstCompile]
public partial struct TitleLogoTriggerJob : ITriggerEventsJob
{
    public ComponentLookup<TitleLogoSingletonData> titleLogoSingletonLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;
    //public ComponentLookup<VFXCreationData> vfxCreationLookup;
    //public ComponentLookup<AudioPlayData> audioPlayLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
        var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

        // entityA��BulletIDealDamageData��L���Ă��邩
        if (!dealDamageLookup.HasComponent(entityA)) { return; }

        // entityB��TitleLogoSingletonData��L���Ă��邩
        if (!titleLogoSingletonLookup.HasComponent(entityB)) { return; }

        var titleLogoSingleton = titleLogoSingletonLookup[entityB];

        // entityB��DestroyableData��L���Ă��邩
        if (!destroyableLookup.HasComponent(entityA)) { return; }

        // �e�̍폜�t���O�𗧂Ă�
        var destroyable = destroyableLookup[entityA];
        destroyable.isKilled = true;
        destroyableLookup[entityA] = destroyable;

        // �^�C�g�����S�̔�eSE���Đ�
        // ���C���X���b�h����Ăяo���ׂ�AudioPlayData���g��
        //AudioManager.Instance.PlaySE(titleLogoSingleton.damageSENumber);

        // �摜��؂�ւ���܂ł̉񐔂��f�N�������g
        titleLogoSingleton.currentImageSwapCount--;

        // �摜��؂�ւ���܂ł̉񐔂�0���傫������
        if (titleLogoSingleton.currentImageSwapCount > 0) 
        {
            // �ύX�𔽉f
            titleLogoSingletonLookup[entityB] = titleLogoSingleton;
            return;
        }

        titleLogoSingleton.currentImageSwapCount = titleLogoSingleton.imageSwapCount;

        // �摜��؂�ւ���
        titleLogoSingleton.isNextImage = true;

        // �ύX�𔽉f
        titleLogoSingletonLookup[entityB] = titleLogoSingleton;
    }
}
