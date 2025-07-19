using UnityEngine;
using UnityEngine.AI;

public class AnimationsCallBacks : MonoBehaviour
{
    private PlayerBase m_Player;

    public void SetPlayer(PlayerBase player)
    {
        m_Player = player;
    }
    private void AttackPlayer()
    {
        m_Player?.DownPlayer();
    }
}
