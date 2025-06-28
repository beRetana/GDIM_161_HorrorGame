using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using StarterAssets;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType{
        MASTER,
        MUSIC,
        AMBIANCE,
        SFX
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (volumeSlider == null)
            volumeSlider = GetComponentInChildren<Slider>();
        LoadSettings();
    }

    private void OnEnable()
    {
        volumeSlider.onValueChanged.AddListener(SetValues);
    }

    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(SetValues);
    }

    private void LoadSettings()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = AudioManager.instance.MasterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioManager.instance.MusicVolume;
                break;
            case VolumeType.AMBIANCE:
                volumeSlider.value = AudioManager.instance.AmbianceVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.instance.SFX_Volume;
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
        }
    }

    public void SetValues(float value)
    {
        switch(volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.instance.MasterVolume = value;
                break;
            case VolumeType.MUSIC:
                AudioManager.instance.MusicVolume = value;
                break;
            case VolumeType.AMBIANCE:
                AudioManager.instance.AmbianceVolume = value;
                break;
            case VolumeType.SFX:
                AudioManager.instance.SFX_Volume = value;
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
        }

        AudioManager.instance.UpdateBuses();
    }
}
