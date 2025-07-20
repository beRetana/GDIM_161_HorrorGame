using UnityEngine;

[RequireComponent (typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator m_PlayerAnimator;
    private const string SPEED = "SPEED";
    private const string JUMP = "JUMP";
    private const string LADDER = "LADDER";
    private const string CRAWL = "CRAWL";
    private const string RAISE_HAND = "RAISE_HAND";
    private const string RIGHT_DOMINANT = "RIGHT_DOMINANT";

    private bool m_IsRaising;

    private void Start()
    {
        m_PlayerAnimator = GetComponent<Animator>();
    }
    public void SetAnimSpeed(float speed)
    {
        m_PlayerAnimator?.SetFloat(SPEED, speed);
    }
    public void SetAnimJump(bool jump)
    {
        m_PlayerAnimator?.SetBool(JUMP, jump);
    }
    public void SetAnimLadder(bool ladder)
    {
        m_PlayerAnimator?.SetBool(LADDER, ladder);
    }
    public void SetAnimCrawl(bool crawl)
    {
        m_PlayerAnimator?.SetBool(CRAWL, crawl);
    }

    public void RaiseHand()
    {
        if (m_IsRaising) return;
        m_PlayerAnimator?.SetTrigger(RAISE_HAND);
        m_IsRaising = true;
    }

    public void DoneRaising()
    {
        m_IsRaising = false;
    }

    public void SwitchHands(bool isRightDominant)
    {
        m_PlayerAnimator?.SetBool(RIGHT_DOMINANT, isRightDominant);
        DoneRaising();
    }
}
