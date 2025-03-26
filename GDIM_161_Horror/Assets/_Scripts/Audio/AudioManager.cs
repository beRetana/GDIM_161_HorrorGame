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
    [Range(0,1)]

    public float masterVolume = 1;
    [Range(0,1)]

    public float musicVolume = 1;
    [Range(0,1)]

    public float ambianceVolume = 1;
    [Range(0,1)]

    public float SFXVolume= 1;

    private Bus masterBus;

    private Bus musicBus;

    private Bus ambianceBus;

    private Bus sfxBus;


    private List<EventInstance> eventInstances;

    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambianceEventInstance;
    public static AudioManager instance {get; private set;}



   
   private void Start()
   {
       InitializeAmbience(FMODEvents.instance.backgroundAmbiance);
   }

    
    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //     {
    //         if (scene.name == "BUILD_1") 
    //             {
    //                 InitializeAmbience(FMODEvents.instance.backgroundAmbiance);    
    //             }
    //     }
    

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError ("Found more than one Audio Manager in the scene");
        }
        instance = this;
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus= RuntimeManager.GetBus("bus:/");
        musicBus= RuntimeManager.GetBus("bus:/Music");
        ambianceBus= RuntimeManager.GetBus("bus:/Ambiance");
        sfxBus= RuntimeManager.GetBus("bus:/SFX");
    }

    
    private void Update()
    {
        {
            masterBus.setVolume(masterVolume);
            musicBus.setVolume(musicVolume);
            ambianceBus.setVolume(ambianceVolume);
            sfxBus.setVolume(SFXVolume);
        }
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
