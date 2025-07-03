using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private StatsBoardUI[] m_StatsBoardUI;
    [SerializeField] private TextMeshProUGUI m_EndGameTitle;

    private void Start()
    {
        foreach (var item in m_StatsBoardUI)
            item.gameObject.SetActive(false);

        gameObject.SetActive(false);
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
        int trialNum = transform.root.GetComponent<PlayerDataTracker>().TrialNumber;
        m_EndGameTitle.text = $"Trial #{trialNum}: {(success ? "Successful" : "Failed")}";
    }

    private void SetStatsBoard()
    {
        PlayerDataTracker[] playersInGame = FindObjectsByType<PlayerDataTracker>(FindObjectsSortMode.None);

        for (byte i = 0; i < playersInGame.Length; ++i)
        {
            m_StatsBoardUI[i].SetPlayerStats(playersInGame[i]);
            m_StatsBoardUI[i].gameObject.SetActive(true);
        }
    }
}
