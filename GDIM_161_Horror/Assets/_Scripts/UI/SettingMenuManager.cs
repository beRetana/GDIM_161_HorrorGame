using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingMenuManager : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;
    public Toggle FullScreenToggle;
    public Slider fovSlider;
    public Camera mainCamera;


    Resolution[] AllResolutions;
    bool IsFullScreen;
    int SelectedResolution;
    List<Resolution> SelectedResolutionList = new List<Resolution>();
   
    void Start()
    {
        IsFullScreen = true;
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }
            
        }


        ResDropDown.AddOptions(resolutionStringList);


        if (fovSlider != null && mainCamera != null)
        {
            fovSlider.minValue = 60f;
            fovSlider.maxValue = 120f;
            fovSlider.value = mainCamera.fieldOfView;
            fovSlider.onValueChanged.AddListener(delegate { ChangeFOV(); });
        }




    }

    public void ChangeResolution()
    {
        SelectedResolution = ResDropDown.value;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);

    }

    public void ChangeFullScreen()
    {
        IsFullScreen = FullScreenToggle.isOn;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);
    }

    public void ChangeFOV()
    {
        if (mainCamera != null && fovSlider != null)
        {
            mainCamera.fieldOfView = fovSlider.value;
        }
    }


    void Update()
    {
        
    }
}
