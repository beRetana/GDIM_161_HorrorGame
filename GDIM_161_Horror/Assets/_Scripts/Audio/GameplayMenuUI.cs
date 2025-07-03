using UnityEngine;
using UnityEngine.InputSystem;
using OtherUtils;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using StarterAssets;

public class GameplayMenuUI : MonoBehaviour, IDebugger
{
    [Header("Components")]
    [SerializeField] private GameObject m_MouseDot;
    [SerializeField] private GameObject m_PauseMenu;
    [SerializeField] private GameObject m_SettingsMenu;
    [SerializeField] private GameObject m_VolumeMenu;

    [Space(5f)]
    [SerializeField] private Button m_BtnReturnToGame;
    [SerializeField] private Button m_BtnPauseToSettings;
    [SerializeField] private Button m_BtnPauseToVolume;
    [SerializeField] private Button m_BtnUnstuckPlayer;
    [SerializeField] private Button m_BtnSurrenderPlayer;

    [Space(5f)]
    [SerializeField] private Button m_BtnSettingReturnToPause;
    [SerializeField] private Button m_BtnVolumeReturnToPause;

    [Space(5f)]
    [SerializeField] private PlayerDataTracker m_PlayerData;

    private PlayerControls m_PlayerControls;
    private FirstPersonController m_FirstPersonController;
    private CharacterController m_CharacterController;
    private NavMeshQueryFilter m_NavMeshQueryFilter;

    private bool m_IsPaused;
    private bool m_Debugger;

    private void Start()
    {
        m_PlayerControls = new();
        m_NavMeshQueryFilter = new NavMeshQueryFilter();
        m_NavMeshQueryFilter.agentTypeID = 0;
        m_NavMeshQueryFilter.areaMask = NavMesh.AllAreas;
        m_CharacterController = m_PlayerData.GetComponent<CharacterController>();
        m_FirstPersonController = m_PlayerData.GetComponent<FirstPersonController>();

        EnableInput();
        SetMenuActive(false);
        SetButtons();
        ChangeCursorState();
    }

    private void OnDestroy()
    {
        DisableInput();
        m_FirstPersonController.OnPlayerUp -= SurrenderState;
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void EnableInput()
    {
        m_PlayerControls.Player.Enable();
        m_PlayerControls.Player.Pause.started += OnPause;
        m_PlayerControls.Player.Pause.canceled += OnPause;
        m_PlayerControls.Player.Pause.performed += OnPause;
    }

    private void DisableInput()
    {
        if (m_PlayerControls == null) return;
        m_PlayerControls.Player.Disable();
        m_PlayerControls.Player.Pause.started -= OnPause;
        m_PlayerControls.Player.Pause.canceled -= OnPause;
        m_PlayerControls.Player.Pause.performed -= OnPause;
    }

    private void SetMenuActive(bool active)
    {
        m_PauseMenu.SetActive(active);
        m_SettingsMenu.SetActive(active);
        m_VolumeMenu.SetActive(active);
    }

    private void SetButtons()
    {
        m_BtnPauseToSettings.onClick.AddListener(OpenSettingsMenu);
        m_BtnPauseToVolume.onClick.AddListener(OpenVolumeMenu);
        m_BtnSettingReturnToPause.onClick.AddListener(CloseSettingsMenu);
        m_BtnVolumeReturnToPause.onClick.AddListener(CloseVolumeMenu);
        m_BtnReturnToGame.onClick.AddListener(ClosePauseMenu);
        m_BtnUnstuckPlayer.onClick.AddListener(UnstuckPlayer);
        m_BtnSurrenderPlayer.onClick.AddListener(Surrender);
        m_BtnSurrenderPlayer.gameObject.SetActive(false);
        m_FirstPersonController.OnPlayerUp += SurrenderState;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                ToggleVolumeMenu();
                break;
            default:
                Debugger($"Action Phase with not purpose {context.phase}");
                break;
        }
    }

    private void ChangeCursorState()
    {
        if (m_IsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void ToggleVolumeMenu()
    {
        if (!m_IsPaused) m_PauseMenu.SetActive(true);
        else SetMenuActive(false);
        m_MouseDot.SetActive(m_IsPaused);
        transform.root.GetComponent<PlayerInput>().enabled = m_IsPaused;
        transform.root.GetComponent<HandInventory>().SetControlsActive(m_IsPaused);
        m_IsPaused = !m_IsPaused;
        ChangeCursorState();
    }

    private void SurrenderState(bool isPlayerUp)
    {
        m_BtnSurrenderPlayer.gameObject.SetActive(!isPlayerUp);
        if (!isPlayerUp) EnableSurrenderBtn();
    }

    private void EnableSurrenderBtn()
    {
        if (m_PlayerData.isServer)
        {
            m_BtnSurrenderPlayer.interactable = true;
        }
        else
        {
            m_BtnSurrenderPlayer.interactable = false;
            m_BtnSurrenderPlayer.
                GetComponentInChildren<TextMeshProUGUI>().text = "Surrender (Waiting for Host)";
        }   
    }

    private void Surrender()
    {
        if (!m_PlayerData.isServer) return;

        transform.parent.GetComponent<PlayerManagerHUD>().SetEndGame(false);
    }

    private void OpenSettingsMenu()
    {
        m_PauseMenu.SetActive(false);
        m_SettingsMenu.SetActive(true);
    }

    private void CloseSettingsMenu()
    { 
        m_PauseMenu.SetActive(true);
        m_SettingsMenu.SetActive(false);
    }

    private void OpenVolumeMenu()
    {
        m_PauseMenu.SetActive(false);
        m_VolumeMenu.SetActive(true);
    }

    private void CloseVolumeMenu()
    {
        m_PauseMenu.SetActive(true);
        m_VolumeMenu.SetActive(false);
    }

    private void ClosePauseMenu()
    {
        ToggleVolumeMenu();
    }

    private void UnstuckPlayer()
    {
        Debugger($"Hitting Unstuck Player");
        NavMeshHit point;
        bool foundPoint = NavMesh.SamplePosition(m_PlayerData.transform.position, out point, 15f, m_NavMeshQueryFilter);
        if (foundPoint)
        {
            Debugger($"Found Walkable Location at {point.position}");
            m_CharacterController.enabled = false;
            m_CharacterController.transform.position = point.position;
            m_CharacterController.enabled = true;
        }
        else
        {
            Debugger($"Resetting to Lastest CheckPoint at {m_PlayerData.SavedPosition}");
            m_CharacterController.enabled = false;
            m_CharacterController.transform.position = m_PlayerData.SavedPosition;
            m_CharacterController.enabled = true;
        }
    }

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{this.GetType().ToString()}] {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}
