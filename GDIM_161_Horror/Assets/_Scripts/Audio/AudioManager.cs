using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem;
using StarterAssets;


public class AudioManager : MonoBehaviour
{

    // [SerializeField] EventReference forestFootstep;
    // [SerializeField] float rate;
    // [SerializeField] GameObject player;
    // [SerializeField] FirstPersonController firstPersonController;

    // float time;

    // public void PlayFootstep()
    // {
    //     RuntimeManager.PlayOneShot(forestFootstep,  player.transform.position);
    // }
    
    // void Update()
    // {
    //     time +=Time.deltaTime;
    //     if (firstPersonController.isWalking)
    //     {
    //         if (time >= rate)
    //             {
    //                 PlayFootstep();
    //                 time = 0;
    //             }
    //     }
    // }

    public static AudioManager instance {get; private set;}

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError ("Found more than one Audio Manager in the scene");
        }
        instance = this;

    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }





}
