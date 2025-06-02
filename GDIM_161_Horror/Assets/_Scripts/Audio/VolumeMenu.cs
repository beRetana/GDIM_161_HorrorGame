using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using StarterAssets;
using Steamworks;

public class VolumeMenu : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject firstSelected;
    public GameObject PauseMenu;
    public GameObject SettingsMenu;
    public GameObject volumeMenu;

    [Header("Player")]
    [SerializeField] private FirstPersonController firstPersonController;

    private bool isPaused = false;

    private void Start()
    {
        menu.gameObject.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
        ForceCursorState();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceCursorState(); 
    }

    private void Update()
    {
        ForceCursorState(); 

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleVolumeMenu();
        }
    }

    private void ForceCursorState()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (menu.gameObject.activeInHierarchy)
        {
           
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (currentScene == "BUILD_1")
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ToggleVolumeMenu()
    {
        isPaused = !isPaused;
        string currentScene = SceneManager.GetActiveScene().name;

        if (isPaused)
        {
            menu.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);

            if (currentScene == "BUILD_1")
            {
                firstPersonController.enabled = false;
            }
        }
        else
        {
            menu.gameObject.SetActive(false);

            if (currentScene == "BUILD_1")
            {
                firstPersonController.enabled = true;
            }
        }
    }

    public void OpenSettings()
    {
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    public void CloseSettings()
    { 
        PauseMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }

    public void OpenVolumeMenu()
    {
        PauseMenu.SetActive(false);
        volumeMenu.SetActive(true);
    }


    public void CloseVolumeMenu()
    {
        PauseMenu.SetActive(true);
        volumeMenu.SetActive(false);
    }

    public void ClosePauseMenu()
    {
        ToggleVolumeMenu();

    }
}
