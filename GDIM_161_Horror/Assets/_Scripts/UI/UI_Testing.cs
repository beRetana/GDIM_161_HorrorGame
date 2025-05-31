using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;
public class UI_Testing : MonoBehaviour
{
    void Start()
    {
        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }
    }

    public void OpenOverlay()
    {
        if (SteamAPI.IsSteamRunning())
        {
            SteamFriends.ActivateGameOverlay("Friends"); // Opens Steam Overlay
        }
        else
        {
            Debug.LogError("Steam is not initialized.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BacktoMain()
    {

        SceneManager.LoadScene(0); 
    }

}
