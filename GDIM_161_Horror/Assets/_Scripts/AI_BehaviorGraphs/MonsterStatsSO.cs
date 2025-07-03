using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Monster Stats")]
public class MonsterStatsSO : ScriptableObject
{
    [SerializeField] private float m_ChaseSpeed;
    [SerializeField] private float m_SearchSpeed;
    [SerializeField] private float m_WanderSpeed;
    [SerializeField] private float m_PavlovSpeed;
    [SerializeField] private float m_RoamingRange;
    [SerializeField] private float m_SightRange;
    [SerializeField] private float m_FireRange;
    [SerializeField] private float m_AttackRange;
    [SerializeField] private float m_SearchRange;
    [SerializeField] private float m_SightAngle;
    [SerializeField] private float m_LowerBound;
    [SerializeField] private float m_UpperBound;
    [SerializeField] private int m_SearchLoops;
    [SerializeField] private int m_WanderLoops;

    public float WanderSpeed => m_WanderSpeed;
    public float ChaseSeed => m_ChaseSpeed;
    public float SearchSpeed => m_SearchSpeed;
    public float PavlovSpeed => m_PavlovSpeed;
    public float RoamingRange => m_RoamingRange;
    public float SightRange => m_SightRange;
    public float FireRange => m_FireRange;
    public int SearchLoops => m_SearchLoops;
    public int WanderLoops => m_WanderLoops;
    public float SightAngle => m_SightAngle;
    public float AttackRange => m_AttackRange;
    public float SearchRange => m_SearchRange;
    public float LowerBound => m_LowerBound;
    public float UpperBound => m_UpperBound;
}
