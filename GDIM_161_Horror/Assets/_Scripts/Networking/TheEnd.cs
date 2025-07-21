using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;

public class TheEnd : NetworkBehaviour
{
    [SerializeField] private float m_EndTime = 5f;

    private List<byte> m_PlayersOut;

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;

        PlayerObjectController player;

        if (!other.transform.root.TryGetComponent<PlayerObjectController>(out player)) return;

        if (m_PlayersOut.Contains((byte)player.PlayerID)) return;

        m_PlayersOut.Add((byte)player.PlayerID);

        if (m_PlayersOut.Count < NewNetworkManager.NewSingleton.numPlayers) return;

        StartCoroutine(EndSequece());
    }

    private IEnumerator EndSequece()
    {
        yield return new WaitForSecondsRealtime(m_EndTime);
        NewNetworkManager.NewSingleton.LoadLobbyScene();
    }
}
