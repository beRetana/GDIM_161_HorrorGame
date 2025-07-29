using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class F_HumanoidMonsterFst : MonoBehaviour
{
    private string EventPath = "event:/Monsters/HumanoidMonster/HMonsterFootstep";

    //void Start()
    //{

    //}
    void PlayHMonsterFootstepEvent()
    {


        EventInstance HMonsterStep = RuntimeManager.CreateInstance(EventPath);
        RuntimeManager.AttachInstanceToGameObject(HMonsterStep, transform, GetComponent<Rigidbody>());

        //Debug.Log("Monster Footstep activated");


        HMonsterStep.start();
        HMonsterStep.release();
    }

}
