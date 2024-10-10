using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;

using TriggerEvent = Unity.Physics.TriggerEvent;

[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var triggerJob = new TriggerJob();
        triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    public partial struct TriggerJob : ITriggerEventsJob
    {
        public void Execute(TriggerEvent triggerEvent)
        {
            Debug.Log($"A�F{triggerEvent.EntityA}, B�F{triggerEvent.EntityB}");
        }
    }

    /*
    public void OnCreate(ref SystemState state)
    {
        // �����V�X�e���̈ˑ��֌W���擾
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // �������[���h�ƃV�~�����[�V�����V�X�e�����擾
        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        // TriggerEventJob�̃X�P�W���[�����O
        var triggerJob = new TriggerEventJob
        {
            MoveComponentLookup = state.GetComponentLookup<PlayerData>(true)
        };

        // �G���e�B�e�BA�̃R���|�[�l���g�̗L�����m�F���A�Y������ꍇ�̂݃W���u���X�P�W���[��
        var triggerEvents = GetTriggerEvents(); // �g���K�[�C�x���g�̃��X�g���擾���郁�\�b�h������
        foreach (var triggerEvent in triggerEvents)
        {
            // entityA �� MoveComponent �������Ă��邩�m�F
            if (triggerJob.MoveComponentLookup.HasComponent(triggerEvent.EntityA))
            {
                state.Dependency = triggerJob.Schedule(triggerEvent, simulation, state.Dependency);
            }
        }

        // TriggerEventJob�̃X�P�W���[�����O
        //var triggerJob = new TriggerEventJob
        //{
            // �K�v�ȃf�[�^�̃Z�b�g�A�b�v
        //};

        // ITriggerEventsJobExtensions.Schedule���g�p����
        //state.Dependency = triggerJob.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    private struct TriggerEventJob : ITriggerEventsJob
    {
        public ComponentLookup<PlayerData> MoveComponentLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            // Trigger���ꂽ���̏���
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            PlayerData moveComponent = MoveComponentLookup[entityA];
        }
    }
    */
}