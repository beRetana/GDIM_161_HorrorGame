using UnityEngine;
using Mirror;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_EndScreen;
    [SerializeField] private ReturnToLobby m_StartScreen;
    [SerializeField] private string m_WinTitle;
    [SerializeField] private string m_LoseTitle;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SetScene(true);
        else if (Input.GetKeyDown(KeyCode.Tab)) SetScene(false);
    }

    private void SetScene(bool won)
    {
        Cursor.lockState = CursorLockMode.None;
        m_EndScreen.SetActive(true);
        if (won) m_StartScreen.SetText(m_WinTitle);
        else m_StartScreen.SetText(m_LoseTitle);
    }
}
