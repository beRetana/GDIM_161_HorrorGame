using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAnimator : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_MaxSpeed = 4.5f;
    
    private NavMeshAgent m_Agent;
    private PlayerBase m_Player;

    private const string SPEED = "SPEED";
    private const string ATTACK = "ATTACK";
    private const string ATTACK_2 = "ATTACK2";
    private float m_MaxSqrSpeed;

    private void Start()
    {
        m_Agent = transform.root.GetComponent<NavMeshAgent>();
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
        StartAttack(player, ATTACK);
    }

    public void TriggerAttackZombie(PlayerBase player)
    {
        StartAttack(player, ATTACK_2);
    }

    private void StartAttack(PlayerBase player, string attack)
    {
        if (m_Player != null) return;

        m_Player = player;
        m_Player.LockPlayer();
        m_Animator.SetTrigger(attack);
    }

    public void Attack()
    {
        m_Player?.DownPlayer();
    }

    public void EndAttack()
    {
        m_Player = null;
    }
}
