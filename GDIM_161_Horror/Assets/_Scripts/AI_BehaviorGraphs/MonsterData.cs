using AI_FSM;
using Mirror;
using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(BehaviorGraphAgent))]
[RequireComponent(typeof(AISightSensor))]
public class MonsterData : NetworkBehaviour
{
    [SerializeField] private MonsterStatsSO m_StatsSO;
    private BehaviorGraphAgent m_Graph;
    private AISightSensor m_Sensor;

    private void Start()
    {
        m_Graph = GetComponent<BehaviorGraphAgent>();
        m_Sensor = GetComponent<AISightSensor>();
        if (isServer) return;
        m_Graph.enabled = false;
        m_Sensor.enabled = false;
    }

    public void SetSearchSpeed(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("SearchSpeed", value);
    }
    public void SetPavlovSpeed(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("PavlovSpeed", value);
    }
    public void SetWanderSpeed(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("WanderSpeed", value);
    }
    public void SetChaseSpeed(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("ChaseSpeed", value);
    }
    public void SetWanderLoops(int value)
    {
        m_Graph.BlackboardReference.SetVariableValue<int>("WanderLoops", value);
    }
    public void SetSearchLoops(int value)
    {
        m_Graph.BlackboardReference.SetVariableValue<int>("SearchLoops", value);
    }
    public void SetAttackRange(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("AttackRange", value);
    }
    public void SetSearchRange(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("SearchRange", value);
    }
    public void SetFireRange(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("FireRange", value);
    }
    public void SetRoamingRange(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("RoamingRange", value);
    }
    public void SetChasePlayerLowerBound(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("ChasePlayerLowerBound", value);
    }
    public void SetChasePlayerUpperBound(float value)
    {
        m_Graph.BlackboardReference.SetVariableValue<float>("ChasePlayerUpperBound", value);
    }
    public void SetSightRange(float value)
    {
        m_Sensor.Range = value;
    }
    public void SetSightAngle(float value)
    {
        m_Sensor.Angle = value;
    }

    public void SetAggressiveStats()
    {
        if (!isServer) return;
        SetSightAngle(m_StatsSO.SightAngle);
        SetSightRange(m_StatsSO.SightRange);
        SetSearchLoops(m_StatsSO.SearchLoops);
        SetPavlovSpeed(m_StatsSO.PavlovSpeed);
        SetWanderLoops(m_StatsSO.WanderLoops);
        SetWanderSpeed(m_StatsSO.WanderSpeed);
        SetSearchSpeed(m_StatsSO.SearchSpeed);
        SetSearchRange(m_StatsSO.SearchRange);
        SetRoamingRange(m_StatsSO.RoamingRange);
        SetChasePlayerLowerBound(m_StatsSO.LowerBound);
        SetChasePlayerUpperBound(m_StatsSO.UpperBound);
        SetAttackRange(m_StatsSO.AttackRange);
    }
}
