using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using OtherUtils;
using UnityEngine.SceneManagement;
using StarterAssets;

public class SettingMenuManager : MonoBehaviour, IDebugger
{
    [SerializeField] private CinemachineCamera m_Camera;

    [Space(5f)]
    [SerializeField] private TMP_Dropdown m_ResDropDown;
    [SerializeField] private TMP_Dropdown m_FOVDropDown;
    [SerializeField] private TMP_Dropdown m_FPSDropDown;
    [SerializeField] private Toggle m_FullScreenToggle;
    [SerializeField] private Button m_BtnSaveChanges;
    [SerializeField] private Slider m_RotationSpeedSlider;
    [SerializeField] private TextMeshProUGUI m_RotationSpeedText;

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
    private const string ROTATION_SPEED = "Rotation_Speed";

    private float m_RotationSpeed;
    private int m_IndexFOV;
    private int m_IndexResolution;
    private int m_IndexFPS;
    private bool m_IsFullScreen;
    private bool m_Debugger;

    private void Awake()
    {
        // Loading saved settings
        LoadSettings();

        QualitySettings.vSyncCount = 0;

        // Setting up UI with saved settings and applying them
        SetUpResolutionsDropDown();
        SetUpFOVDropDown();
        SetUpFPSDropDown();
        SetUpFullScreen();
        SetUpRotationSpeed();
    }

    private void LoadSettings()
    {
        Debugger("Loading Saved Settings");
        m_IsFullScreen = PlayerPrefs.GetInt(FULL_SCREEN, 1) == 1 ? true : false;
        m_IndexFOV = PlayerPrefs.GetInt(FOV, 4);
        ChangeFPS(PlayerPrefs.GetInt(FPS, 5));
        m_IndexResolution = PlayerPrefs.GetInt(RESOLUTION, -1);
        m_RotationSpeed = PlayerPrefs.GetFloat(ROTATION_SPEED, .5f);
    }

    private void SetUpFullScreen()
    {
        ApplyChangeFullScreen(); // Apply saved Changes
        m_FullScreenToggle.isOn = m_IsFullScreen; // Update UI with saved changes
    }

    private void SetUpFPSDropDown()
    {
        // Load in dropdown with resolution options
        List<string> options = new List<string>();

        foreach (int fps in m_fpsSettings)
        {
            if (fps == -1) options.Add("Unlimited");
            else options.Add(fps.ToString());
        }

        m_FPSDropDown.AddOptions(options);
        m_FPSDropDown.value = m_IndexFPS; // Set saved choice.
        Application.targetFrameRate = m_fpsSettings[m_IndexFPS]; // Apply choice.
    }

    private void SetUpResolutionsDropDown()
    {
        List<string> options = new List<string>();
        int defResolution = 0;
        foreach (Resolution res in Screen.resolutions)
        {
            string newRes = res.width.ToString() + " x " + res.height.ToString();
            if (res.height == Screen.height && res.width == Screen.width && m_IndexResolution == -1)
                m_IndexResolution = defResolution;
            if (options.Contains(newRes)) continue;

            options.Add(newRes);
            m_ResolutionSettings.Add(res);
            ++defResolution;
        }

        m_ResDropDown.AddOptions(options);
        
        if (m_IndexResolution != -1)
            ApplyChangeResolution();
        else
        {
            ApplyChangeFullScreen();
        }
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

    private void ChangeRotationSpeed(float value)
    {
        m_RotationSpeed = value;
        m_RotationSpeedText.text = (Mathf.Round(m_RotationSpeed * 1000) / 10).ToString();
    }

    private void SetUpRotationSpeed()
    {
        ApplyChangeRotationSpeed();
    }

    public void ApplyChangeResolution()
    {
        Debugger($"Applying changes of Resolution to " +
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
        Debugger($"Applying Changing to {(m_IsFullScreen ? "Full Screen" : "Windowed")}");
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

    private void ApplyChangeRotationSpeed()
    {
        Debugger($"Applying Changes to Rotation Speed ");

        m_RotationSpeedSlider.value = m_RotationSpeed;
        m_RotationSpeedText.text = (Mathf.Round(m_RotationSpeed * 1000) / 10).ToString();
        if (!transform.root.TryGetComponent<FirstPersonController>(out var player)) return;
        player.SetCameraRotationSpeed(m_RotationSpeed);

        PlayerPrefs.SetFloat(ROTATION_SPEED, m_RotationSpeed);
    }

    private void SaveSettings()
    {
        ApplyChangeFullScreen();
        ApplyChangeFOV();
        ApplyChangeResolution();
        ApplyChangeFPS();
        ApplyChangeRotationSpeed();
        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        m_BtnSaveChanges.onClick.AddListener(SaveSettings);
        m_ResDropDown.onValueChanged.AddListener((int value) => { m_IndexResolution = value; });
        m_FOVDropDown.onValueChanged.AddListener((int value) => { m_IndexFOV = value; });
        m_FullScreenToggle.onValueChanged.AddListener((bool value) => { m_IsFullScreen = value; });
        m_FPSDropDown.onValueChanged.AddListener(ChangeFPS);
        m_RotationSpeedSlider.onValueChanged.AddListener(ChangeRotationSpeed);
    }

    private void OnDisable()
    {
        m_ResDropDown.onValueChanged.RemoveAllListeners();
        m_FOVDropDown.onValueChanged.RemoveAllListeners();
        m_FPSDropDown.onValueChanged.RemoveAllListeners();
        m_FullScreenToggle.onValueChanged.RemoveAllListeners();
        m_BtnSaveChanges.onClick.RemoveAllListeners();
        m_RotationSpeedSlider.onValueChanged.RemoveListener(ChangeRotationSpeed);
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
