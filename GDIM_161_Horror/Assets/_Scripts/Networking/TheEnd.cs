using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;

public class TheEnd : NetworkBehaviour
{
    [SerializeField] private float m_EndTime = 5f;

    private List<byte> m_PlayersOut;

    private void Start()
    {
        m_PlayersOut = new List<byte>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;

        PlayerObjectController player;
        if (!other.transform.root.TryGetComponent<PlayerObjectController>(out player)) return;
        
        if (m_PlayersOut.Contains((byte)player.PlayerID)) return;

        m_PlayersOut.Add((byte)player.PlayerID);

        if (m_PlayersOut.Count < NewNetworkManager.NewSingleton.numPlayers) return;

        StartSequence();
    }

    [ClientRpc]
    private void StartSequence()
    {
        Debug.Log("Client - Ending Coroutine");
        StartCoroutine(EndSequece());
    }

    private IEnumerator EndSequece()
    {
        Debug.Log("Client - Waiting");
        yield return new WaitForSecondsRealtime(m_EndTime);

        SetCredits(true); 
        Debug.Log("Client - Credits on");
        yield return new WaitForSecondsRealtime(m_EndTime);
        Debug.Log("Client - Credits off");
        SetCredits(false);
        EndTheGame();
    }

    private void SetCredits(bool active)
    {
        PlayerManagerHUD[] playerHUDs = FindObjectsByType<PlayerManagerHUD>(FindObjectsSortMode.None);

        foreach (PlayerManagerHUD player in playerHUDs)
        {
            player.SetCredits(active);
        }
    }

    private void EndTheGame()
    {
        GameplayMenuHUD[] players = FindObjectsByType<GameplayMenuHUD>(FindObjectsSortMode.None);

        foreach (GameplayMenuHUD player in players)
        {
            if (!isServer) continue;
            player.StartGameOverSetUp(true);
        }
    }
}
