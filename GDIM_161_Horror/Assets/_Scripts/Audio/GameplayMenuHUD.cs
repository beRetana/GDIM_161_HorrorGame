using UnityEngine;
using UnityEngine.InputSystem;
using OtherUtils;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using StarterAssets;
using System.Collections;
using Mirror;

public class GameplayMenuHUD : NetworkBehaviour, IDebugger
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
    [SerializeField] private TextMeshProUGUI m_TxtSurrenderPlayer;

    [Space(5f)]
    [SerializeField] private Button m_BtnSettingReturnToPause;
    [SerializeField] private Button m_BtnVolumeReturnToPause;

    [Space(5f)]
    [SerializeField] private PlayerDataTracker m_PlayerData;

    private PlayerControls m_PlayerControls;
    private PlayerInput m_PlayerInput;
    private FirstPersonController m_FirstPersonController;
    private CharacterController m_CharacterController;
    private HandInventory m_HandInventory;
    private NavMeshQueryFilter m_NavMeshQueryFilter;

    [SyncVar] private byte m_PlayersDown;
    [SyncVar] private bool m_Surrended;
    [SyncVar] private bool m_GameEnded;
    
    private byte m_TotalPlayers;
    private bool m_IsPaused;
    private bool m_Debugger;

    public byte PlayersDown { get { return m_PlayersDown; } set {  m_PlayersDown = value; } }
    
    private void Start()
    {
        m_NavMeshQueryFilter = new NavMeshQueryFilter();
        m_NavMeshQueryFilter.agentTypeID = 0;
        m_NavMeshQueryFilter.areaMask = NavMesh.AllAreas;
    }

    private void OnDestroy()
    {
        DisableInput();
        DisableButtons();
    }

    public void DisableMenuUI()
    {
        DisableInput();
        SetMenuActive(false);
        DisableButtons();
        ChangeCursorState(true);
        m_MouseDot.SetActive(false);
        gameObject.SetActive(false);
    }

    public void EnableMenuUI()
    {
        m_CharacterController = m_PlayerData.GetComponent<CharacterController>();
        m_FirstPersonController = m_PlayerData.GetComponent<FirstPersonController>();
        m_HandInventory = m_PlayerData.GetComponent<HandInventory>();
        m_PlayerInput = m_PlayerData.GetComponent<PlayerInput>();

        gameObject.SetActive(true);
        m_MouseDot.SetActive(true);
        ChangeCursorState(false);
        EnableButtons();
        SetMenuActive(false);
        EnableInput();
    }

    private void EnableInput()
    {
        if (m_PlayerControls == null) m_PlayerControls = new();
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

    public void SetMenuActive(bool active)
    {
        m_PauseMenu.SetActive(active);
        m_SettingsMenu.SetActive(active);
        m_VolumeMenu.SetActive(active);
    }

    private void EnableButtons()
    {
        m_BtnPauseToSettings.onClick.AddListener(OpenSettingsMenu);
        m_BtnPauseToVolume.onClick.AddListener(OpenVolumeMenu);
        m_BtnSettingReturnToPause.onClick.AddListener(CloseSettingsMenu);
        m_BtnVolumeReturnToPause.onClick.AddListener(CloseVolumeMenu);
        m_BtnReturnToGame.onClick.AddListener(ClosePauseMenu);
        m_BtnUnstuckPlayer.onClick.AddListener(UnstuckPlayer);
        m_BtnSurrenderPlayer.onClick.AddListener(Surrender);
        
        if (m_FirstPersonController == null) 
            m_FirstPersonController = transform.root.GetComponent<FirstPersonController>();
        m_FirstPersonController.OnPlayerUp += GameState;
        GetTotalPlayers();
        UpdateSurrenderText();
    }

    public void ResetSurrender()
    {
        m_PlayersDown = 0;
        m_Surrended = false;
        m_GameEnded = false;
    }

    private void GetTotalPlayers()
    {
        PlayerBase[] playerList = FindObjectsByType<PlayerBase>(FindObjectsSortMode.None);
        m_TotalPlayers = (byte) playerList.Length;
    }

    private void DisableButtons()
    {
        m_BtnPauseToSettings.onClick.RemoveListener(OpenSettingsMenu);
        m_BtnPauseToVolume.onClick.RemoveListener(OpenVolumeMenu);
        m_BtnSettingReturnToPause.onClick.RemoveListener(CloseSettingsMenu);
        m_BtnVolumeReturnToPause.onClick.RemoveListener(CloseVolumeMenu);
        m_BtnReturnToGame.onClick.RemoveListener(ClosePauseMenu);
        m_BtnUnstuckPlayer.onClick.RemoveListener(UnstuckPlayer);
        m_BtnSurrenderPlayer.onClick.RemoveListener(Surrender);
        m_FirstPersonController.OnPlayerUp -= GameState;
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

    public void ChangeCursorState(bool freeMouse)
    {
        Cursor.lockState = (freeMouse) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = freeMouse;
    }

    public void ResetMenu()
    {
        ToggleVolumeMenu();
    }

    public void ToggleVolumeMenu()
    {
        if (!m_PlayerData.isLocalPlayer) return;

        Debugger($"The current state is paused: {m_IsPaused} the new state is pause: {!m_IsPaused}");
        if (!m_IsPaused) m_PauseMenu.SetActive(true);
        else SetMenuActive(false);
        SetControls(m_IsPaused);
        m_IsPaused = !m_IsPaused;
        ChangeCursorState(m_IsPaused);
    }

    public void SetControls(bool active)
    {
        m_MouseDot.SetActive(active);
        m_PlayerInput.enabled = active;
        m_HandInventory.EnablePickingUp = active;
        m_HandInventory.SetControlsActive(active);
    }

    private void GameState(bool isPlayerUp)
    {
        if (!isLocalPlayer) return;

        if (isServer) RpcPlayersDownCount(m_Surrended);
        else          CmdPlayersDownCount(m_Surrended);
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayersDownCount(bool isPlayerUp)
    {
        RpcPlayersDownCount(isPlayerUp);
    }

    [ClientRpc]
    private void RpcPlayersDownCount(bool isPlayerUp)
    {
        GameplayMenuHUD[] gameplayMenuHUDs = FindObjectsByType<GameplayMenuHUD>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        m_Surrended = !isPlayerUp;
        m_TotalPlayers = (byte)gameplayMenuHUDs.Length;

        foreach (GameplayMenuHUD gameplayMenuHUD in gameplayMenuHUDs)
        {
            if (m_Surrended) ++gameplayMenuHUD.PlayersDown;
            else --gameplayMenuHUD.PlayersDown;

            gameplayMenuHUD.UpdateSurrenderText();
        }

        if (!UpdateSurrenderText() || m_GameEnded) return;
        
        m_Surrended = false;
        m_GameEnded = true;
        m_PlayersDown = 0;

        Debugger($"Starting Surrender");
        if (isServer) StartGameOverSetUp(false);
        else CmdStartGameOverSetUp();
    }

    private void Surrender()
    {
        GameState(m_Surrended);
    }

    private bool UpdateSurrenderText()
    {
        bool isMajority = m_PlayersDown > m_TotalPlayers / 2;
        m_TxtSurrenderPlayer.text = $"Surrender ({m_PlayersDown}/{m_TotalPlayers})";
        m_TxtSurrenderPlayer.color = (isMajority) ? Color.green : Color.red;
        return isMajority;
    }

    [Command(requiresAuthority = false)]
    private void CmdStartGameOverSetUp()
    {
        Debugger($"CMD - Starting Surrender");
        StartGameOverSetUp(false);
    }

    [Server]
    public void StartGameOverSetUp(bool won)
    {
        GameplayMenuHUD[] gameplayMenuHUDs = FindObjectsByType<GameplayMenuHUD>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (GameplayMenuHUD gameplayMenuHUD in gameplayMenuHUDs)
        {
            gameplayMenuHUD.ResetSurrender();
            if (!gameplayMenuHUD.isLocalPlayer) continue;
            Debugger($"SERVER - Starting ROUTINE");
            gameplayMenuHUD.OpenVolumeMenu();
            gameplayMenuHUD.ChangeCursorState(true);
            gameplayMenuHUD.StartCoroutine(won);
        }
    }

    public void StartCoroutine(bool won)
    {
        StartCoroutine(SurrenderSetUp(won));
    }

    private IEnumerator SurrenderSetUp(bool won)
    {
        m_PlayerData.EndGame();
        
        yield return new WaitForSecondsRealtime(.2f);
        
        PlayerManagerHUD[] playerManagerHUDs = FindObjectsByType<PlayerManagerHUD>(FindObjectsSortMode.None);
        
        for (byte i = 0; i < playerManagerHUDs.Length; ++i)
        {
            playerManagerHUDs[i].SetEndGame(won);
        }

        m_GameEnded = false;
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
        Debugger("CLOSING PAUSE MENU");
        ToggleVolumeMenu();
    }

    private void UnstuckPlayer()
    {
        Debugger($"Hitting Unstuck Player");
        NavMeshHit point;
        bool foundPoint = NavMesh.SamplePosition(m_PlayerData.transform.position, out point, 7f, m_NavMeshQueryFilter);
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
