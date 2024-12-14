using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using static EntityCampsHelper;
using static HealthPointDatas;
using static BulletHelper;
using Unity.Physics;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// EnemyTag��L����Entity�̏���
/// </summary>
[BurstCompile]
public partial struct EnemySystem : ISystem
{
    private ComponentLookup<HealthPointData> _healthPointLookup;
    private ComponentLookup<BulletIDealDamageData> _dealDamageLookup;
    private ComponentLookup<DestroyableData> _destroyableLookup;
    private ComponentLookup<RemainingPierceCountData> _remainingPierceCountLookup;
    private ComponentLookup<LocalTransform> _localTransformLookup;
    private ComponentLookup<VFXCreationData> _vfxCreationLookup;
    private ComponentLookup<AudioPlayData> _audioPlayLookup;
    private ComponentLookup<BossEnemyCampsTag> _bossEnemyCampsLookup;

    public void OnCreate(ref SystemState state)
    {
        // �擾����
        _healthPointLookup = state.GetComponentLookup<HealthPointData>(false);
        _dealDamageLookup = state.GetComponentLookup<BulletIDealDamageData>(false);
        _destroyableLookup = state.GetComponentLookup<DestroyableData>(false);
        _remainingPierceCountLookup = state.GetComponentLookup<RemainingPierceCountData>(false);
        _localTransformLookup = state.GetComponentLookup<LocalTransform>(false);
        _vfxCreationLookup = state.GetComponentLookup<VFXCreationData>(false);
        _audioPlayLookup = state.GetComponentLookup<AudioPlayData>(false);
        _bossEnemyCampsLookup = state.GetComponentLookup<BossEnemyCampsTag>(false);
    }

    public void OnUpdate(ref SystemState state)
    {
        // MovementRangeSingletonData�����݂��Ȃ�����
        if (!SystemAPI.HasSingleton<MovementRangeSingletonData>()) { return; }

        // �V���O���g���f�[�^�̎擾
        var movementRangeSingleton = SystemAPI.GetSingleton<MovementRangeSingletonData>();

        // �G�̈ړ��\�͈͂��擾
        var enemyMovementRange = movementRangeSingleton.EnemyMovementRange;

        // ���S�ʒu
        var movementRangeCenter = enemyMovementRange.movementRangeCenter;

        // �����̑傫�������߂�
        var halfMovementRange = enemyMovementRange.movementRange.Halve();

        // ���S�ʒu���l�������ړ��\�͈͂����߂�
        var minMovementRange = movementRangeCenter + -halfMovementRange;
        var maxMovementRange = movementRangeCenter + halfMovementRange;

        // �ړ��\�͈͊O��������폜�t���O�𗧂Ă�
        foreach (var (enemy, destroyable, localTfm) in
                 SystemAPI.Query
                 <RefRW<EnemyCampsTag>,
                 RefRW<DestroyableData>,
                 RefRW<LocalTransform>>())
        {
            var position = localTfm.ValueRO.Position;

            // �ړ��\�͈͊O������
            if (position.x < minMovementRange.x || position.x > maxMovementRange.x ||
                position.y < minMovementRange.y || position.y > maxMovementRange.y ||
                position.z < minMovementRange.z || position.z > maxMovementRange.z)
            {
                // �폜�t���O�𗧂Ă�
                destroyable.ValueRW.isKilled = true;
            }
        }

        // �G�̔�e����
        EnemyDamageTriggerJob(ref state);
    }

    /// <summary>
    /// �G�̔�e�������Ăяo��
    /// </summary>
    /// <param name="state"></param>
    private void EnemyDamageTriggerJob(ref SystemState state)
    {
        // �X�V����
        _healthPointLookup.Update(ref state);
        _dealDamageLookup.Update(ref state);
        _destroyableLookup.Update(ref state);
        _remainingPierceCountLookup.Update(ref state);
        _localTransformLookup.Update(ref state);
        _vfxCreationLookup.Update(ref state);
        _audioPlayLookup.Update(ref state);
        _bossEnemyCampsLookup.Update(ref state);

        int frameCount = Time.frameCount;

        // �G�ɒe�������������̏������Ăяo��
        var enemyDamage = new EnemyDamageTriggerJob()
        {
            healthPointLookup = _healthPointLookup,
            dealDamageLookup = _dealDamageLookup,
            destroyableLookup = _destroyableLookup,
            remainingPierceCountLookup = _remainingPierceCountLookup,
            localTransformLookup = _localTransformLookup,
            vfxCreationLookup = _vfxCreationLookup,
            audioPlayLookup = _audioPlayLookup,
            bossEnemyCampsLookup = _bossEnemyCampsLookup,
            frameCount = frameCount
        };

        // �O�̃W���u����������
        state.Dependency.Complete();

        var enemyJobHandle = enemyDamage.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        // �W���u�̈ˑ��֌W���X�V����
        state.Dependency = enemyJobHandle;
    }
}

/// <summary>
/// �ڐG���Ƀ_���[�W���󂯂�
/// </summary>
public partial struct EnemyDamageTriggerJob : ITriggerEventsJob
{
    // �C���X�^���X�̎擾�ɕK�v�ȕϐ�
    public ComponentLookup<HealthPointData> healthPointLookup;
    public ComponentLookup<BulletIDealDamageData> dealDamageLookup;
    public ComponentLookup<DestroyableData> destroyableLookup;
    public ComponentLookup<RemainingPierceCountData> remainingPierceCountLookup;
    public ComponentLookup<LocalTransform> localTransformLookup;
    public ComponentLookup<VFXCreationData> vfxCreationLookup;
    public ComponentLookup<AudioPlayData> audioPlayLookup;
    public ComponentLookup<BossEnemyCampsTag> bossEnemyCampsLookup;

    [Tooltip("�ڐG�����t���[��")]
    public int frameCount;

    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA; // �ڐG�Ώ�
        var entityB = triggerEvent.EntityB; // isTrigger��L���ɂ��Ă����

        // entityB��LocalTransform��L���Ă��邩
        if (!localTransformLookup.HasComponent(entityB)) { return; }

        var localTransform = localTransformLookup[entityB];
        var position = localTransform.Position;

        // entityA��BulletIDealDamageData��L���Ă��Ȃ��B
        // ���邢�́AentityB��EnemyHealthPointData��L���Ă��Ȃ���ΐ؂�グ��
        if (!dealDamageLookup.HasComponent(entityA) || !healthPointLookup.HasComponent(entityB)) { return; }

        // entityB����EnemyHealthPointData���擾
        var healthPoint = healthPointLookup[entityB];

        // entityA����BulletIDealDamageData���擾
        var dealDamage = dealDamageLookup[entityA];

        // �_���[�W���̐w�c�̎�ނ�Enemy��������؂�グ��
        if (dealDamage.campsType == EntityCampsType.Enemy) { return; }

        // �_���[�W��^���A�ύX���ꂽ�C���X�^���X�𔽉f����
        healthPoint = dealDamage.DealDamage(healthPoint, entityA, frameCount);
        healthPointLookup[entityB] = healthPoint;

        // �_���[�W����RemainingPierceCountData��L���Ă���
        if (remainingPierceCountLookup.HasComponent(entityA))
        {
            // �c��ђʉ񐔂��f�N�������g
            var remainingPierceCount = remainingPierceCountLookup[entityA];
            remainingPierceCount.remainingPierceCount--;
            remainingPierceCountLookup[entityA] = remainingPierceCount;

            // �_���[�W����DestroyableData��L���Ă���
            if (destroyableLookup.HasComponent(entityA))
            {
                var destroyable = destroyableLookup[entityA];

                // �c��ђʉ񐔂�0�ȉ���
                destroyable.isKilled = (remainingPierceCount.remainingPierceCount <= 0) ? true : false;

                // �ύX�𔽉f
                destroyableLookup[entityA] = destroyable;
            }
        }

        // �U�����󂯂��Ώۂ�DestroyableData��L���Ă���
        if (destroyableLookup.HasComponent(entityB))
        {
            var destroyable = destroyableLookup[entityB];

            // HP�̍폜�t���O��������
            destroyable.isKilled = healthPoint.CurrentHP <= 0;

            // �ύX�𔽉f
            destroyableLookup[entityB] = destroyable;

            // EntityB��AudioPlayData��L���Ă���
            if (audioPlayLookup.HasComponent(entityB))
            {
                var audioPlay = audioPlayLookup[entityB];

                // �폜�t���O�������Ă����玀�S���̌��ʉ��A�����Ă��Ȃ���Δ�e���̌��ʉ�����
                audioPlay.AudioNumber = (destroyable.isKilled) ? healthPoint.killedSENumber : healthPoint.hitSENumber;

                audioPlayLookup[entityB] = audioPlay;
            }

            // �|���ꂽ
            if (destroyable.isKilled)
            {
                // EntityB��VFXCreationData��L���Ă���
                if (vfxCreationLookup.HasComponent(entityB))
                {
                    var vfxCreation = vfxCreationLookup[entityB];

                    vfxCreation.Position = position;
                    vfxCreationLookup[entityB] = vfxCreation;
                }

                // �{�X�G������
                if (bossEnemyCampsLookup.HasComponent(entityB))
                {
                    // �{�X���|���ꂽ���߃Q�[���N���A
                    GameManager.Instance.MyGameState = GameState.GameClear;
                }
            }
        }
    }
}