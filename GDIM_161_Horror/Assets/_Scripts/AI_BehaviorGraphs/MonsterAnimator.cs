using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class MonsterAnimator : NetworkBehaviour
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
        if (!isServer) enabled = false;
        m_Agent = transform.root.GetComponent<NavMeshAgent>();
        m_MaxSqrSpeed = Mathf.Pow(m_MaxSpeed, 2);
    }

    private void Update()
    {
        if (!isServer) return;
        SetSpeedAnim();
    }

    [Server]
    private void SetSpeedAnim()
    {
        m_Animator.SetFloat(SPEED, Mathf.Abs(m_Agent.velocity.sqrMagnitude / m_MaxSqrSpeed));
    }

    [Server]
    public void TriggerAttackBasic(PlayerBase player)
    {
        StartAttack(player, ATTACK);
    }

    [Server]
    public void TriggerAttackZombie(PlayerBase player)
    {
        StartAttack(player, ATTACK_2);
    }

    [Server]
    private void StartAttack(PlayerBase player, string attack)
    {
        if (m_Player != null) return;

        m_Player = player;
        //m_Player.LockPlayer();
        m_Animator.ResetTrigger(attack);
        m_Animator.SetTrigger(attack);
    }

    public void Attack()
    {
        if (!isServer) return;
        m_Player?.DownPlayer();
    }

    public void EndAttack()
    {
        if (!isServer) return;
        m_Player = null;
    }
}
