using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using OtherUtils;
using UnityEngine.SceneManagement;

public class SettingMenuManager : MonoBehaviour, IDebugger
{
    [SerializeField] private TMP_Dropdown m_ResDropDown;
    [SerializeField] private TMP_Dropdown m_FOVDropDown;
    [SerializeField] private TMP_Dropdown m_FPSDropDown;
    [SerializeField] private Toggle m_FullScreenToggle;
    [SerializeField] private Button m_BtnSaveChanges;

    [Space(5f)]
    [SerializeField] private List<int> m_fpsSettings = 
            new List<int> { 30, 60, 120, 144, 240, -1 };
    [SerializeField] private List<int> m_fovSettings = 
            new List<int> { 40, 50, 60, 70, 80, 90, 100, 110, 120 };

    private CinemachineCamera m_Camera;
    private List<Resolution> m_ResolutionSettings;

    private const string FULL_SCREEN = "Full_Screen";
    private const string RESOLUTION = "Resolution";
    private const string FPS = "FPS";
    private const string FOV = "FOV";

    private string m_GameScene;
    private bool m_IsFullScreen;
    private bool m_Debugger;
    private int m_SelectedFOV;
    private int m_SelectedResolution;
    private int m_SelectedFPS;

    private void Awake()
    {
        LoadSettings();
    }

    void Start()
    {
        m_Camera = GameObject.Find("PlayerFollowCamera")?.GetComponent<CinemachineCamera>();
        Debugger($"The player camera was {(m_Camera != null ? "Found" : "Not Found")}");
        
        m_ResolutionSettings = new List<Resolution>();
        m_GameScene = (NewNetworkManager.singleton as NewNetworkManager).GetOnlineSceneName();

        SetUpResolutionsDropDown();
        SetUpFOVDropDown();
        SetUpFPSDropDown();
        SetUpFullScreen();

        m_BtnSaveChanges.onClick.AddListener(SaveSettings);
    }

    private void LoadSettings()
    {
        m_IsFullScreen = PlayerPrefs.GetInt(FULL_SCREEN, 0) == 1 ? true : false;
        m_SelectedFOV = PlayerPrefs.GetInt(FOV, 0);
        m_SelectedFPS = PlayerPrefs.GetInt(FPS, 0);
        m_SelectedResolution = PlayerPrefs.GetInt(RESOLUTION, 0);
    }

    private void SetUpFullScreen()
    {
        Screen.SetResolution(m_ResolutionSettings[m_SelectedResolution].width,
            m_ResolutionSettings[m_SelectedResolution].height,
            m_IsFullScreen);

        m_FullScreenToggle.isOn = m_IsFullScreen;
        m_FullScreenToggle.onValueChanged.AddListener((bool value) => { m_IsFullScreen = value; });
    }

    private void SetUpFPSDropDown()
    {
        List<string> options = new List<string>();

        foreach (int fps in m_fpsSettings)
        {
            if (fps == -1) options.Add("Unlimited");
            else options.Add(fps.ToString());
        }

        m_FPSDropDown.AddOptions(options);
        m_FPSDropDown.value = m_fpsSettings[m_SelectedFPS];
        m_FPSDropDown.onValueChanged.AddListener((int value) => { m_SelectedFPS = value; });
        Application.targetFrameRate = m_SelectedFPS;
    }

    private void SetUpResolutionsDropDown()
    {
        List<string> options = new List<string>();

        foreach (Resolution res in Screen.resolutions)
        {
            string newRes = res.width.ToString() + " x " + res.height.ToString();
            
            if (options.Contains(newRes)) continue;

            options.Add(newRes);
            m_ResolutionSettings.Add(res);
        }

        m_ResDropDown.AddOptions(options);
        m_ResDropDown.value = m_SelectedResolution;
        ApplyChangeResolution();
        m_ResDropDown.onValueChanged.AddListener((int value) => { m_SelectedResolution = value; });
    }

    private void SetUpFOVDropDown()
    {
        List<string> options = new List<string>();

        foreach (int res in m_fovSettings)
            options.Add(res.ToString());

        m_FOVDropDown.AddOptions(options);
        m_FOVDropDown.value = m_fovSettings[m_SelectedFOV];
        ApplyChangeFOV();
        m_FOVDropDown.onValueChanged.AddListener((int value) => { m_SelectedFOV = value; });
    }

    public void ApplyChangeResolution()
    {
        Debugger($"Changing Resolution to " +
                 $"{m_ResolutionSettings[m_SelectedResolution].width} x " +
                 $"{m_ResolutionSettings[m_SelectedResolution].height}; " +
                 $"{(m_IsFullScreen ? "Full Screen" : "Windowed")}");

        Screen.SetResolution(m_ResolutionSettings[m_SelectedResolution].width, 
                             m_ResolutionSettings[m_SelectedResolution].height, 
                             m_IsFullScreen);

        PlayerPrefs.SetInt(RESOLUTION, m_SelectedResolution);
    }

    public void ApplyChangeFullScreen()
    {
        Debugger($"Changing to {(m_IsFullScreen ? "Full Screen" : "Windowed")}");
        Screen.SetResolution(m_ResolutionSettings[m_SelectedResolution].width,
            m_ResolutionSettings[m_SelectedResolution].height,
            m_IsFullScreen);

        PlayerPrefs.SetInt(FULL_SCREEN, m_IsFullScreen ? 1:0);
    }

    public void ApplyChangeFOV()
    {
        Debugger($"Changing FOV to: {m_SelectedFOV}");
        if (SceneManager.GetActiveScene().name == m_GameScene)
        {
            if (m_Camera == null) Camera.main.fieldOfView = m_SelectedFOV;
            else m_Camera.Lens.FieldOfView = m_SelectedFOV;
        }

        PlayerPrefs.SetInt(FOV, m_SelectedFOV);
    }

    private void ApplyChangeFPS()
    {
        Application.targetFrameRate = m_fpsSettings[m_SelectedFPS];
        PlayerPrefs.SetInt(FPS, m_SelectedResolution);
    }

    private void SaveSettings()
    {
        ApplyChangeFullScreen();
        ApplyChangeFOV();
        ApplyChangeResolution();
        ApplyChangeFPS();
        PlayerPrefs.Save();
    }

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{this.GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}
