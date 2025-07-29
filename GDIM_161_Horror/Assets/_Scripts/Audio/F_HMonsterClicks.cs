using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class F_HMonsterClicks : MonoBehaviour
{
    private string EventPath = "event:/Monsters/HumanoidMonster/HMonsterClicks";

    //void Start()
    //{

    //}
    void PlayHMonsterClicksEvent()
    {


        EventInstance HMonsterClicks = RuntimeManager.CreateInstance(EventPath);
        RuntimeManager.AttachInstanceToGameObject(HMonsterClicks, transform, GetComponent<Rigidbody>());

        //Debug.Log("Monster Look Around activated");


        HMonsterClicks.start();
        HMonsterClicks.release();
    }

}
