using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;
using OtherUtils;

public class TheEnd : NetworkBehaviour, IDebugger
{
    [SerializeField] private float m_CreditsTime = 15f;
    [SerializeField] private float m_EndTime = 3f;

    private HashSet<byte> m_PlayersOut;
    private bool m_Debugger;

    private void Start()
    {
        m_PlayersOut = new HashSet<byte>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;
        Debugger(other.gameObject.name);
        if (!other.transform.root.TryGetComponent<PlayerObjectController>(out var player)) return;
        
        if (!m_PlayersOut.Add((byte)player.PlayerID)) return;

        if (m_PlayersOut.Count < NewNetworkManager.NewSingleton.numPlayers) return;

        StartSequence();
    }

    [ClientRpc]
    private void StartSequence()
    {
        Debugger("Client - Ending Coroutine");
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        Debugger("Client - Waiting");
        yield return new WaitForSecondsRealtime(m_EndTime);

        foreach(var playerID in m_PlayersOut)
        {
            PlayerBase player = PlayerManager.Instance.GetPlayer(playerID);
            player.LockPlayer();
            player.SetInputState(false);
        }

        SetCredits(true); 
        Debugger("Client - Credits on");
        yield return new WaitForSecondsRealtime(m_CreditsTime);
        Debugger("Client - Credits off");
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

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}
