using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAnimator : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private AnimationsCallBacks m_CallBacks;
    [SerializeField] private float m_MaxSpeed = 4.5f;
    private NavMeshAgent m_Agent;

    private const string SPEED = "SPEED";
    private const string ATTACK = "ATTACK";
    private const string ATTACK_2 = "ATTACK2";
    private float m_MaxSqrSpeed;

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_MaxSqrSpeed = Mathf.Pow(m_MaxSpeed, 2);
    }

    private void Update()
    {
        SetSpeedAnim();
    }

    private void SetSpeedAnim()
    {
        m_Animator.SetFloat(SPEED, Mathf.Abs(m_Agent.velocity.sqrMagnitude / m_MaxSqrSpeed));
    }

    public void TriggerAttackBasic(PlayerBase player)
    {
        m_CallBacks.SetPlayer(player);
        m_Animator.SetTrigger(ATTACK);
    }

    public void TriggerAttackZombie(PlayerBase player)
    {
        m_CallBacks.SetPlayer(player);
        m_Animator.SetTrigger(ATTACK_2);
    }
}
