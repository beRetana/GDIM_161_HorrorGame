using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using TMPro;
using OtherUtils;

public class EndGameHUD : MonoBehaviour, IDebugger
{
    [SerializeField] private StatsBoardUI[] m_StatsBoardUI;
    [SerializeField] private TextMeshProUGUI m_EndGameTitle;
    private bool m_Debugger;
    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log(log);
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }

    public void SetEndGameUI(bool won)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetEndGameTittle(won);
        SetStatsBoard();
        gameObject.SetActive(true);
    }

    private void SetEndGameTittle(bool success)
    {
        ushort trialNum = transform.root.GetComponent<PlayerDataTracker>().TrialNumber;
        Debugger($"Player {transform.root.gameObject.name} has trial number {trialNum}");
        m_EndGameTitle.text = $"Trial #{trialNum}: {(success ? "Successful" : "Failed")}";
    }

    private void SetStatsBoard()
    {
        PlayerDataTracker[] playersInGame = FindObjectsByType<PlayerDataTracker>(FindObjectsSortMode.None);

        for (byte i = 0; i < playersInGame.Length; ++i)
        {
            Debugger($"Player {playersInGame[i].gameObject.name} for stats");
            playersInGame[i].EndGame();
            m_StatsBoardUI[i].SetPlayerStats(playersInGame[i]);
            m_StatsBoardUI[i].gameObject.SetActive(true);
        }
    }
}
