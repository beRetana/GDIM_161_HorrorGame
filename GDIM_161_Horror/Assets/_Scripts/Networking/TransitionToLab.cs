using UnityEngine;
using Mirror;
using Interactions;
using System.Collections.Generic;
using System.Collections;

public class TransitionToLab : NetworkBehaviour
{
    [SerializeField] private DoubleDoor m_DoubleDoor;
    [SerializeField] private Collider m_Collider;
    [SerializeField] private Transform m_Text;
    [SerializeField] private float m_WaitingTime = 5f;

    private HashSet<byte> m_PlayerIDs;
    private const int REACHED_LAB = 4;

    private void Start()
    {
        m_PlayerIDs = new HashSet<byte>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;

        PlayerObjectController player;
        if (!other.TryGetComponent<PlayerObjectController>(out player)) return;

        if (!m_PlayerIDs.Add((byte)player.PlayerID)) return;
        player.GetComponent<PlayerDataTracker>().OnReachedNewFloor(REACHED_LAB);
        if (m_PlayerIDs.Count < NewNetworkManager.NewSingleton.numPlayers) return;

        StartCoroutine(MovingToLab());
    }

    private IEnumerator MovingToLab()
    {
        m_Collider.isTrigger = false;
        m_DoubleDoor.LockingDoors();
        yield return new WaitForSeconds(m_WaitingTime);
        m_Text.gameObject.SetActive(true); 
        yield return new WaitForSeconds(m_WaitingTime);
        NewNetworkManager.NewSingleton.LoadLabScene();
    }
}
