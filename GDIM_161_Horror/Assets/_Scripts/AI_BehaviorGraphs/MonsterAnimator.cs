using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimator : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float m_MaxSpeed = 4.5f;
    private NavMeshAgent m_Agent;

    private const string SPEED = "SPEED";
    private const string JUMP = "JUMP";
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

    public void TriggerJump()
    {
        m_Animator.SetTrigger(JUMP);
    }
}
