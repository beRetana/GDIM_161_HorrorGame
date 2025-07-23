using Mirror;
using UnityEngine;
using OtherUtils;

public class PlayersDistanceTracker : NetworkBehaviour, IDebugger
{
    [SerializeField] private Activator[] m_Activators;
    [SerializeField] private float m_ChecksPerSecond;

    private Transform[] m_PlayerTransforms;
    private float m_Frequency;
    private float m_Timer;
    private bool m_Debugger;
    private bool m_PlayersReady;

    private void OnEnable()
    {
        NewNetworkManager.NewSingleton.OnPlayersServerReady += SetUp;
    }

    private void OnDisable()
    {
        NewNetworkManager.NewSingleton.OnPlayersServerReady -= SetUp;
    }

    private void SetUp()
    {
        if (!isServer) return;
        Debug.Log("Setting Up Activator");
        m_Frequency = 1f / m_ChecksPerSecond;
        GetPlayers();
        m_PlayersReady = true;
    }

    [Server]
    private void GetPlayers()
    {
        PlayerObjectController[] players = FindObjectsByType<PlayerObjectController>(FindObjectsSortMode.None);
        m_PlayerTransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; ++i)
        {
            m_PlayerTransforms[i] = players[i].transform;
        }
    }

    private void Update()
    {
        if (!m_PlayersReady) return;
        if (m_Timer >= m_Frequency)
        {
            m_Timer = 0f;
            UpdateActivators();
        }
        m_Timer += Time.deltaTime;
    }

    [Server]
    private void UpdateActivators()
    {
        Vector3[] positions = new Vector3[m_PlayerTransforms.Length];

        for (int i = 0; i < positions.Length; ++i)
        {
            if (m_PlayerTransforms[i] == null) continue; 
            positions[i] = m_PlayerTransforms[i].position;
        }

        foreach (Activator activator in m_Activators)
        {
            activator.UpdateObjectsState(positions);
        }
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
