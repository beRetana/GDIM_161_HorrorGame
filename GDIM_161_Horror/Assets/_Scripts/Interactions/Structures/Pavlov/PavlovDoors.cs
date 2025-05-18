using UnityEngine;

public class PavlovDoors : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    private const string OPEN_DOOR = "OPEN_DOOR";
    private const string CLOSE_DOOR = "CLOSE_DOOR";

    public void OpenDoors()
    {
        m_Animator.SetTrigger(OPEN_DOOR);
    }

    public void CloseDoors()
    {
        m_Animator.SetTrigger(CLOSE_DOOR);
    }
}
