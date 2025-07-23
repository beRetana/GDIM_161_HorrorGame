using Mirror;
using OtherUtils;
using System;
using System.Collections;
using UnityEngine;

public class LadderManager : NetworkBehaviour, IDebugger
{
    [SerializeField] private Ladder[] m_Ladders;
    [SerializeField, Range(0f, 1f), Tooltip("Highest probability of choosing the closest ladder")]
    private float m_UpperBoundProbability;
    [SerializeField] private bool m_SpawnAtStart;

    private Transform m_ClosestLadder;
    private float m_ClosestProbability;
    private bool m_OnlyOneLadder;
    private bool m_Debug;

    private void OnEnable()
    {
        NewNetworkManager.NewSingleton.OnPlayersServerReady += SetUpLadders;

        if (NewNetworkManager.NewSingleton.PlayersReady)
        {
            SetUpLadders();
        }
    }

    private void OnDisable()
    {
        NewNetworkManager.NewSingleton.OnPlayersServerReady -= SetUpLadders;
    }

    [Server]
    private void SetUpLadders()
    {
        StartCoroutine(StartLate());
    }

    private IEnumerator StartLate()
    {
        yield return null;
        DeactivateLadders();
        CalculateProbabilities();
        if (m_SpawnAtStart)
            ActivateLadders(Vector3.zero);
    }

    public void PlayerCheckedIn(Vector3 playerPosition)
    {
        if (!isServer) CmdActivateLadders(playerPosition);
        else ActivateLadders(playerPosition);
    }

    [Command(requiresAuthority = false)]
    public void CmdActivateLadders(Vector3 playerPosition)
    {
        ActivateLadders(playerPosition);
    }

    [Server]
    private void ActivateLadders(Vector3 playerPosition)
    {
        GetClosestLadder(playerPosition);
        float[] numbers = new float[m_Ladders.Length];

        for(int i = 0; i < m_Ladders.Length; ++i)
        {
            float upperBound = (m_Ladders[i] == m_ClosestLadder) ? m_ClosestProbability : 1f;
            float prob = UnityEngine.Random.Range(0f, upperBound);
            numbers[i] = prob;
            Debugger($"For index {i} the number was {prob}");
        }

        Debugger($"Setting the first ladder active");
        ServerSetLadderActive(GetHighest(numbers));

        if (m_OnlyOneLadder) return;
        Debugger($"Setting the Second ladder active");
        ServerSetLadderActive(GetHighest(numbers));
    }

    [Server]
    private void ServerSetLadderActive(int index)
    {
        Debugger("Setting Ladder active in Server");
        RpcSetLadderActive(index);
    }

    [ClientRpc]
    public void RpcSetLadderActive(int index)
    {
        Debugger("Setting Ladder active in Client");
        m_Ladders[index].SetLadderActive(true);
    }

    private int GetHighest(float[] array)
    {
        float highest = 0;
        int index = 0;

        for (int i = 0; i < array.Length; ++i)
        {
            if (array[i] <= highest) continue;

            highest = array[i];
            index = i;
        }

        Debugger($"The highest value is: {highest} with index {index}");

        array[index] = 0;
        return index;
    }

    private void CalculateProbabilities()
    {
        m_ClosestProbability = UnityEngine.Random.Range(0, m_UpperBoundProbability);
        m_OnlyOneLadder = UnityEngine.Random.value < .5f ? true : false;
    }

    private void GetClosestLadder(Vector3 playerPosition)
    {
        float closestDistance = float.MaxValue;
        foreach (Ladder ladder in m_Ladders)
        {
            float distanceSqr = (ladder.transform.position - playerPosition).sqrMagnitude;
            Debugger($"Distance Squared for {ladder.gameObject} is {distanceSqr}");
            if (distanceSqr > closestDistance) continue;
            closestDistance = distanceSqr;
            m_ClosestLadder = ladder.transform;
        }
    }

    [ClientRpc]
    private void DeactivateLadders()
    {
        Debugger("Deactivating all ladders");
        foreach(Ladder ladder in m_Ladders)
        {
            ladder.SetLadderActive(false);
        }
    }

    public void Debugger(object log)
    {
        if (m_Debug) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debug = active;
    }
}
