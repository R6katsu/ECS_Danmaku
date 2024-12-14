using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Random = UnityEngine.Random;
using static EntityCampsHelper;


#if UNITY_EDITOR
using Unity.Physics;
using static SpawnPointSingletonData;
using System.Linq;
using System.Linq.Expressions;
using Unity.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using static MoveHelper;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
#endif

// ���t�@�N�^�����O�ς�

/// <summary>
/// �G�̐�������
/// </summary>
[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    [Tooltip("�傫���̏����l")]
    private const float DEFAULT_SCALE = 1.0f;

    [Tooltip("���ݒ�̒l")]
    private const int UNSET_VALUE = int.MinValue;

    private EnemySpawnPattern _currentPattern;
    private int _currentInfoNumber;
    private float _elapsed;

    [Tooltip("�{�X�����܂ł̃J�E���g�_�E��")]
    private int _countdownBossSpawn;

    public void OnUpdate(ref SystemState state)
    {
        // EnemySpawnPatternArraySingletonData�����݂��Ă��Ȃ�����
        if (!SystemAPI.HasSingleton<EnemySpawnPatternArraySingletonData>())
        {
            // ������
            Initialize();
        }

        // �o�ߎ��ԂɃt���[���b�����Z���
        _elapsed += SystemAPI.Time.DeltaTime;

        // �ꎞ�I��Buffer���쐬
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // �G�����݂��邩
        bool hasEnemy = HasEnemy(ref state);

        // Entity��Data���擾����
        foreach (var (array, entity)
            in SystemAPI.Query<EnemySpawnPatternArraySingletonData>()
            .WithEntityAccess())
        {
            // �p�^�[���̐���0�ȉ�������
            if (array.enemySpawnPatterns.Length <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError("�G�𐶐�����p�^�[�������݂��Ȃ�");
#endif
                continue;
            }

            // �{�X�����܂ł̃J�E���g�_�E�������ݒ�Ȃ珉����
            _countdownBossSpawn = (_countdownBossSpawn == UNSET_VALUE) ? array.countdownBossSpawn : _countdownBossSpawn;

            // _currentPattern��default�A
            // �܂���enemySpawnInfos�̒�����_currentInfoNumber�����߂��Ă����珉����
            if (_currentPattern.Equals(default) 
                || _currentPattern.enemySpawnInfos.Length <= _currentInfoNumber)
            {
                // EnemyTag������Entity��1�̈ȏ㑶�݂���ꍇ�̓R���e�j���[
                if (hasEnemy) { continue; }

                // ���ɐ�������G�p�^�[���𒊑I
                var patternNumber = Random.Range(0, array.enemySpawnPatterns.Length);
                _currentPattern = array.enemySpawnPatterns[patternNumber];

                // �{�X�����܂ł̃J�E���g�_�E�����f�N�������g
                _countdownBossSpawn--;

                // ������
                _currentInfoNumber = 0;
                _elapsed = 0.0f;
            }

            // �{�X�����܂ł̃J�E���g�_�E���� 0�ȉ��ɂȂ���
            if (_countdownBossSpawn <= 0)
            {
                // �{�X�G�𐶐�
                BossEnemyInstantiate(ref state, ecb, array);

                // EnemySpawnPatternArraySingletonData�����݂��Ă���
                if (SystemAPI.HasSingleton<EnemySpawnPatternArraySingletonData>())
                {
                    // EnemySpawnPatternArraySingletonData���A�^�b�`���ꂽEntity����폜����
                    var enemySpawnPatternArraySingletonDataEntity = SystemAPI.GetSingletonEntity<EnemySpawnPatternArraySingletonData>();
                    ecb.RemoveComponent<EnemySpawnPatternArraySingletonData>(enemySpawnPatternArraySingletonDataEntity);
                }
                break;
            }

            // �G�𐶐�
            EnemyInstantiate(ref state, ecb, array);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    /// <summary>
    /// EnemyName�ɑΉ�����GEntity���擾����
    /// </summary>
    /// <param name="enemyName">�GEntity�̖���</param>
    /// <returns>EnemyName�ɑΉ�����GEntity</returns>
    private Entity GetEnemyEntity(ref SystemState systemState, EnemyName enemyName)
    {
        foreach (var enemyEntityData in SystemAPI.Query<EnemyEntityData>())
        {
            if (enemyEntityData.enemyName == enemyName)
            {
                return enemyEntityData.enemyEntity;
            }
        }

        // ������Ȃ�����
        return Entity.Null;
    }

    /// <summary>
    /// �G�����݂��邩�̃t���O��Ԃ�
    /// </summary>
    /// <returns>�G�����݂��邩</returns>
    private bool HasEnemy(ref SystemState state)
    {
        foreach (var enemyTag in SystemAPI.Query<EnemyCampsTag>())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �{�X�G�𐶐�
    /// </summary>
    /// <param name="state">SystemAPI�ɕK�v</param>
    /// <param name="ecb">�ύX�̔��f�ɕK�v</param>
    /// <param name="array">�G�����ɕK�v�Ȃ���</param>
    private void BossEnemyInstantiate(ref SystemState state, EntityCommandBuffer ecb, EnemySpawnPatternArraySingletonData array)
    {
        // �{�X�𐶐�
        var bossEnemy = ecb.Instantiate(array.bossEnemyEntity);

        // SpawnPointSingletonData�����݂��Ă���
        if (SystemAPI.HasSingleton<SpawnPointSingletonData>())
        {
            // �V���O���g���f�[�^�̎擾
            var spawnPointSingleton = SystemAPI.GetSingleton<SpawnPointSingletonData>();

            // �����ʒu���擾
            var spawnPoint = spawnPointSingleton.GetSpawnPoint(SpawnPointType.Center);

            // null�`�F�b�N�Bnull�������猴�_�ɐ���
            spawnPoint = (spawnPoint == null) ? float3.zero : spawnPoint;

            // LocalTransform��ݒ肷��
            ecb.SetComponent(bossEnemy, new LocalTransform
            {
                Position = (float3)spawnPoint,
                Rotation = quaternion.identity,
                Scale = DEFAULT_SCALE
            });
        }
    }

    /// <summary>
    /// �G�𐶐�
    /// </summary>
    /// <param name="state">SystemAPI�ɕK�v</param>
    /// <param name="ecb">�ύX�̔��f�ɕK�v</param>
    /// <param name="array">�G�����ɕK�v�Ȃ���</param>
    private void EnemyInstantiate(ref SystemState state, EntityCommandBuffer ecb, EnemySpawnPatternArraySingletonData array)
    {
        // ���݂�enemySpawnInfo���擾
        var currentInfo = _currentPattern.enemySpawnInfos[_currentInfoNumber];

        // �o�ߎ��Ԃ��G�������Ԗ�����������R���e�j���[
        if (currentInfo.CreationDelay > _elapsed) { return; }

        // �������̓G�ԍ����C���N�������g
        _currentInfoNumber++;

        // EnemyName�ɑΉ�����GEntity���擾
        var enemyEntity = GetEnemyEntity(ref state, currentInfo.MyEnemyName);

        // �GEntity��Null��������R���e�j���[
        if (enemyEntity == Entity.Null) { return; }

        // �G�𐶐�
        var enemy = ecb.Instantiate(enemyEntity);

        // SpawnPointSingletonData�����݂��Ă���
        if (SystemAPI.HasSingleton<SpawnPointSingletonData>())
        {
            // �V���O���g���f�[�^�̎擾
            var spawnPointSingleton = SystemAPI.GetSingleton<SpawnPointSingletonData>();

            // �����ʒu���擾
            var spawnPoint = spawnPointSingleton.GetSpawnPoint
            (
                currentInfo.SpawnPointType
            );

            // null�`�F�b�N�Bnull�������猴�_�ɐ���
            spawnPoint = (spawnPoint == null) ? float3.zero : spawnPoint;

            // LocalTransform��ݒ肷��
            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = (float3)spawnPoint,
                Rotation = quaternion.identity,
                Scale = DEFAULT_SCALE
            });
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Initialize()
    {
        _currentPattern = new();
        _elapsed = 0.0f;
        _currentInfoNumber = 0;
        _countdownBossSpawn = UNSET_VALUE;
    }
}
