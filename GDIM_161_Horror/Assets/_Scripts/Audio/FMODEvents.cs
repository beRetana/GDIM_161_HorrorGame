using System.Collections;
using System.Collections. Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("Torch SFX")]
    [field: SerializeField] public EventReference torchGrab {get; private set;}

    [field: Header("backgroundAmbiance")]
    [field: SerializeField] public EventReference backgroundAmbiance {get; private set;}

    [field: Header("deerWalk")]
    [field: SerializeField] public EventReference deerWalk {get; private set;}

     [field: Header("deerRun")]
    [field: SerializeField] public EventReference deerRun {get; private set;}

    [field: Header("sonarPing")]
    [field: SerializeField] public EventReference sonarPing {get; private set;}

    [field: Header("walkieStart")]
    [field: SerializeField] public EventReference walkieStart { get; private set; }

    [field: Header("walkieEnd")]
    [field: SerializeField] public EventReference walkieEnd { get; private set; }

    [field: Header("radioStatic")]
    [field: SerializeField] public EventReference radioStatic { get; private set; }

    [field: Header("doorsOpening")]
    [field: SerializeField] public EventReference doorsOpening { get; private set; }

    [field: Header("doorsClosing")]
    [field: SerializeField] public EventReference doorsClosing { get; private set; }


    //Example for future one shot referrences
    //AudioManager.instance.PlayOneShot(FMODEvents.instance.sonarPing, this.transform.position);
    public static FMODEvents instance {get; private set;}

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events scripts in the scene");
        }
        instance = this;
    }
}
