using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimator : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    private NavMeshAgent m_Agent;

    private const string SPEED = "SPEED";
    private const string JUMP = "JUMP";

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        SetSpeedAnim();
    }

    private void SetSpeedAnim()
    {
        m_Animator.SetFloat(SPEED, Mathf.Clamp01(m_Agent.speed));
    }

    public void TriggerJump()
    {
        m_Animator.SetTrigger(JUMP);
    }
}
