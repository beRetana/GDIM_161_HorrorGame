using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Unity.MLAgents;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomLocation", story: "[Agent] roams for [Max] loops in a range of [Range]", category: "Action/Navigation", id: "12d486f3a15efa731fa7e2a474a9953d")]
public partial class RandomLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Max;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1f);
    [SerializeReference] public BlackboardVariable<float> DistanceBuffer = new BlackboardVariable<float>(1f);
    [SerializeReference] public BlackboardVariable<float> StoppingWaitTime = new BlackboardVariable<float>(0.2f);
    [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SPEED");
    [SerializeReference] public BlackboardVariable<string> AnimatorSearchingParam = new BlackboardVariable<string>("SEARCHING");

    protected NavMeshAgent m_NavMeshAgent;
    protected Animator m_Animator;
    protected float m_PreviusStoppingDistance;
    protected float m_WaitingTimer;
    protected int m_LoopNumber;
    protected bool m_Waiting;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            LogFailure("No agent assigned.");
            return Status.Failure;
        }

        Initialize();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (m_LoopNumber == 0) return Status.Success;
        
        if (Agent.Value == null ) return Status.Failure;

        if (m_Animator != null)
        {
            m_Animator.SetFloat(AnimatorSpeedParam, m_NavMeshAgent.velocity.magnitude);
        }

        if (m_Waiting) StartWaiting();
        else CheckForDistance();

        return Status.Running;
    }

    protected override void OnDeserialize()
    {
        base.OnDeserialize();
        Initialize();
    }

    protected virtual void Initialize()
    {
        m_NavMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
        m_Animator = Agent.Value.GetComponent<Animator>();
        if (m_NavMeshAgent == null || !m_NavMeshAgent.isActiveAndEnabled || !m_NavMeshAgent.isOnNavMesh)
        {
            LogFailure("No Navmesh Agent found or Inactive!");
            return;
        }

        if (m_NavMeshAgent.isOnNavMesh) m_NavMeshAgent.ResetPath();
        m_NavMeshAgent.speed = Speed.Value;
        m_PreviusStoppingDistance = m_NavMeshAgent.stoppingDistance;
        m_NavMeshAgent.stoppingDistance = DistanceBuffer.Value;
        m_LoopNumber = Max;
    }

    protected override void OnEnd()
    {
        if (m_Animator != null) m_Animator.SetFloat(AnimatorSpeedParam, 0);

        if (m_NavMeshAgent == null) return;

        if (m_NavMeshAgent.isOnNavMesh) m_NavMeshAgent.ResetPath();
        m_NavMeshAgent.stoppingDistance = m_PreviusStoppingDistance;
    }

    protected virtual void CheckForDistance()
    {
        if (m_NavMeshAgent.remainingDistance > DistanceBuffer) return;
        if (m_Animator != null)
        {
            m_Animator.SetFloat(AnimatorSpeedParam, 0);
            m_Animator.SetBool(AnimatorSearchingParam, true);
        }
        m_WaitingTimer = StoppingWaitTime;
        m_Waiting = true;
    }

    protected virtual void StartWaiting()
    {
        if (m_WaitingTimer > 0.0f) m_WaitingTimer -= Time.deltaTime;
        
        else
        {
            m_WaitingTimer = 0f;
            m_Waiting = false;
            if (m_Animator != null) m_Animator.SetBool(AnimatorSearchingParam, false);
            --m_LoopNumber;
            MoveToRandomLocation();
        }
    }

    protected virtual void MoveToRandomLocation()
    {
        for (int tries = 0; tries < 10; ++tries)
        {
            Vector3 randomLocation = Agent.Value.transform.position + UnityEngine.Random.insideUnitSphere * Range;
            if (NavMesh.SamplePosition(randomLocation, out NavMeshHit hit, Range, NavMesh.AllAreas))
            {
                m_NavMeshAgent.SetDestination(randomLocation);
                return;
            }
        }
    }
}

