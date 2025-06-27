using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem;
using StarterAssets;
using UnityEngine.SceneManagement;
using FMOD.Studio;


public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0,1)] private float masterVolume = 1;
    [Range(0,1)] private float musicVolume = 1;
    [Range(0,1)] private float ambianceVolume = 1;
    [Range(0,1)] private float SFXVolume = 1;

    private const string MASTER_VOLUME = "MasterVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string AMBIANCE_VOLUME = "AmbianceVolume";
    private const string SFX_VOLUME = "SFX_Volume";

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambianceBus;
    private Bus sfxBus;

    public float MasterVolume {get { return masterVolume; } 
        set 
        { 
            masterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MASTER_VOLUME, masterVolume);
            PlayerPrefs.Save();
        } }
    public float MusicVolume {get { return musicVolume; } 
        set 
        { 
            musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);
            PlayerPrefs.Save();
        } }
    public float AmbianceVolume { get { return ambianceVolume; } 
        set 
        { 
            ambianceVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(AMBIANCE_VOLUME, ambianceVolume);
            PlayerPrefs.Save();
        } }
    public float SFX_Volume { get { return SFXVolume; } 
        set 
        { 
            SFXVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(SFX_VOLUME, SFXVolume);
            PlayerPrefs.Save();
        } }

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambianceEventInstance;
    public static AudioManager instance {get; private set;}

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError ("Found more than one Audio Manager in the scene");
            Destroy(this.gameObject);
        }
        else instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambianceBus = RuntimeManager.GetBus("bus:/Ambiance");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        LoadSettings();
    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.instance.backgroundAmbiance);
        UpdateBuses();
    }

    private void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1f);
        ambianceVolume = PlayerPrefs.GetFloat(AMBIANCE_VOLUME, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
    }

    public void UpdateBuses()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambianceBus.setVolume(ambianceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    private void InitializeAmbience(EventReference ambianceEventReference )
    {
        ambianceEventInstance = CreateInstance(ambianceEventReference);
        ambianceEventInstance.start();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        //stop all the event emitters, bc if not, it could hang around for future scene changes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
