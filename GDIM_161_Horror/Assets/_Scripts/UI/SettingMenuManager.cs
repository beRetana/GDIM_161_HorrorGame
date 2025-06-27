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
    [SerializeField] private CinemachineCamera m_Camera;

    [Space(5f)]
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

    private List<Resolution> m_ResolutionSettings = new List<Resolution>();

    private const string FULL_SCREEN = "Full_Screen";
    private const string RESOLUTION = "Resolution";
    private const string FPS = "FPS";
    private const string FOV = "FOV";

    private string m_GameScene;
    private bool m_IsFullScreen;
    private bool m_Debugger;
    private int m_IndexFOV;
    private int m_IndexResolution;
    private int m_IndexFPS;

    private void Awake()
    {
        LoadSettings();

        QualitySettings.vSyncCount = 0;

        SetUpResolutionsDropDown();
        SetUpFOVDropDown();
        SetUpFPSDropDown();
        SetUpFullScreen();
    }

    private void LoadSettings()
    {
        m_IsFullScreen = PlayerPrefs.GetInt(FULL_SCREEN, 0) == 1 ? true : false;
        m_IndexFOV = PlayerPrefs.GetInt(FOV, 0);
        ChangeFPS(PlayerPrefs.GetInt(FPS, 0));
        m_IndexResolution = PlayerPrefs.GetInt(RESOLUTION, 0);
    }

    private void SetUpFullScreen()
    {
        Screen.SetResolution(m_ResolutionSettings[m_IndexResolution].width,
            m_ResolutionSettings[m_IndexResolution].height,
            m_IsFullScreen);

        m_FullScreenToggle.isOn = m_IsFullScreen;
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
        m_FPSDropDown.value = m_IndexFPS;
        Application.targetFrameRate = m_fpsSettings[m_IndexFPS];
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
        m_ResDropDown.value = m_IndexResolution;
        ApplyChangeResolution();
    }

    private void SetUpFOVDropDown()
    {
        List<string> options = new List<string>();

        foreach (int res in m_fovSettings)
            options.Add(res.ToString());

        m_FOVDropDown.AddOptions(options);
        m_FOVDropDown.value = m_IndexFOV;
        ApplyChangeFOV();
    }

    private void ChangeFPS(int value)
    {
        Debugger($"The value given to is {value}");
        if (value > m_fpsSettings.Count-1) m_IndexFPS = m_fpsSettings.Count-1;
        else if (0 > value) m_IndexFPS = 0;
        else m_IndexFPS = value;
    }

    public void ApplyChangeResolution()
    {
        Debugger($"Changing Resolution to " +
                 $"{m_ResolutionSettings[m_IndexResolution].width} x " +
                 $"{m_ResolutionSettings[m_IndexResolution].height}; " +
                 $"{(m_IsFullScreen ? "Full Screen" : "Windowed")}");

        Screen.SetResolution(m_ResolutionSettings[m_IndexResolution].width, 
                             m_ResolutionSettings[m_IndexResolution].height, 
                             m_IsFullScreen);

        PlayerPrefs.SetInt(RESOLUTION, m_IndexResolution);
    }

    public void ApplyChangeFullScreen()
    {
        Debugger($"Changing to {(m_IsFullScreen ? "Full Screen" : "Windowed")}");
        Screen.SetResolution(m_ResolutionSettings[m_IndexResolution].width,
            m_ResolutionSettings[m_IndexResolution].height,
            m_IsFullScreen);

        PlayerPrefs.SetInt(FULL_SCREEN, m_IsFullScreen ? 1:0);
    }

    public void ApplyChangeFOV()
    {
        Debugger($"Changing FOV to: {m_fovSettings[m_IndexFOV]}");

        PlayerPrefs.SetInt(FOV, m_IndexFOV);

        Debugger($"The player camera was {(m_Camera != null ? "Found" : "Not Found")}");

        if (m_Camera == null) return;
        
        m_Camera.Lens.FieldOfView = m_fovSettings[m_IndexFOV];
    }

    private void ApplyChangeFPS()
    {
        Debugger($"Changing FPS to: {m_fpsSettings[m_IndexFPS]}");
        Application.targetFrameRate = m_fpsSettings[m_IndexFPS];
        PlayerPrefs.SetInt(FPS, m_IndexFPS);
    }

    private void SaveSettings()
    {
        ApplyChangeFullScreen();
        ApplyChangeFOV();
        ApplyChangeResolution();
        ApplyChangeFPS();
        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        m_BtnSaveChanges.onClick.AddListener(SaveSettings);
        m_ResDropDown.onValueChanged.AddListener((int value) => { m_IndexResolution = value; });
        m_FOVDropDown.onValueChanged.AddListener((int value) => { m_IndexFOV = value; });
        m_FullScreenToggle.onValueChanged.AddListener((bool value) => { m_IsFullScreen = value; });
        m_FPSDropDown.onValueChanged.AddListener(ChangeFPS);
    }

    private void OnDisable()
    {
        m_ResDropDown.onValueChanged.RemoveAllListeners();
        m_FOVDropDown.onValueChanged.RemoveAllListeners();
        m_FPSDropDown.onValueChanged.RemoveAllListeners();
        m_FullScreenToggle.onValueChanged.RemoveAllListeners();
        m_BtnSaveChanges.onClick.RemoveAllListeners();
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
