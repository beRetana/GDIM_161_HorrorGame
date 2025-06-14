using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class F_MonsterDeerFst : MonoBehaviour
{
    private string EventPath = "event:/Monsters/monsterDeerFootstep";

    //void Start()
    //{

    //}
    void PlayMDeerFootstepEvent()
    {


        EventInstance MonsterStep = RuntimeManager.CreateInstance(EventPath);
        RuntimeManager.AttachInstanceToGameObject(MonsterStep, transform, GetComponent<Rigidbody>());




        MonsterStep.start();
        MonsterStep.release();
    }

}
