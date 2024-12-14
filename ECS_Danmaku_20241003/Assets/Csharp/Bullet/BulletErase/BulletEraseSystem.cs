using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using static BulletHelper;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using static EntityCampsHelper;
using static HealthPointDatas;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �e�������E�\�̏���
/// </summary>
[BurstCompile]
public partial struct BulletEraseSystem : ISystem
{
    private ComponentLookup<BulletEraseTag> _bulletEraseLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;

    public void OnCreate(ref SystemState state)
    {
        // �擾����
        _bulletEraseLookup = state.GetComponentLookup<BulletEraseTag>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        // �X�V����
        _bulletEraseLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);

        // �e�������E�\�̏Փˎ��̏������Ăяo��
        var bulletErase = new BulletEraseTriggerJob()
        {
            bulletEraseLookup = _bulletEraseLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup
        };

        // �O�̃W���u����������
        state.Dependency.Complete();

        var bulletEraseJobHandle = bulletErase.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = bulletEraseJobHandle;
    }
}

/// <summary>
/// �e�������E�\�̏Փˎ��̏���
/// </summary>
[BurstCompile]
public partial struct BulletEraseTriggerJob : ITriggerEventsJob
{
    public ComponentLookup<BulletEraseTag> bulletEraseLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
        var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

        // entityB��BulletEraseTag��L���Ă��Ȃ��B
        if (!bulletEraseLookup.HasComponent(entityB)) { return; }

        // entityA��BulletIDealDamageData��L���Ă��Ȃ��B
        if (!dealDamageLookup.HasComponent(entityA)) { return; }

        // entityA����BulletIDealDamageData���擾
        var dealDamage = dealDamageLookup[entityA];

        // entityB��BulletIDealDamageData��L���Ă���
        if (dealDamageLookup.HasComponent(entityB))
        {
            // entityB����BulletIDealDamageData���擾
            var dealDamageEntityB = dealDamageLookup[entityB];

            // �����w�c��������؂�グ��
            if (dealDamage.campsType == dealDamageEntityB.campsType) { return; }
        }

        // �ڐG�Ώۂ�DestroyableData��L���Ă���
        if (!destroyableLookup.HasComponent(entityA)) { return; }

        // �ڐG�Ώۂ̍폜�t���O�𗧂Ă�
        var destroyable = destroyableLookup[entityA];
        destroyable.isKilled = true;
        destroyableLookup[entityA] = destroyable;
    }
}
