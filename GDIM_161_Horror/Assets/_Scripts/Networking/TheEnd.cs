using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;

public class TheEnd : NetworkBehaviour
{
    [SerializeField] private float m_CreditsTime = 15f;
    [SerializeField] private float m_EndTime = 3f;

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
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        Debug.Log("Client - Waiting");
        yield return new WaitForSecondsRealtime(m_EndTime);

        foreach(var playerID in m_PlayersOut)
        {
            PlayerBase player = PlayerManager.Instance.GetPlayer(playerID);
            player.LockPlayer();
            player.SetInputState(false);
        }

        SetCredits(true); 
        Debug.Log("Client - Credits on");
        yield return new WaitForSecondsRealtime(m_CreditsTime);
        Debug.Log("Client - Credits off");
        SetCredits(false);
        EndTheGame();
    }

    private void SetCredits(bool active)
    {
        GameplayMenuHUD.ActOnAllPlayers((PlayerManagerHUD playerHUD) => playerHUD.SetCredits(active));
    }

    private void EndTheGame()
    {
        if (!isServer) return;

        GameplayMenuHUD.ActOnAllPlayers((GameplayMenuHUD player) => player.StartGameOverSetUp(true));
    }
}
