using UnityEngine;

public class PavlovStand : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    private const string DELIVER_FOOD = "Deliver";
    private const string HIDE_FOOD = "Hide";

    public void DeliverFood()
    {
        m_Animator.SetTrigger(DELIVER_FOOD);
    }

    public void HideFood()
    {
        m_Animator.SetTrigger(HIDE_FOOD);
    }
}
