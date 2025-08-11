using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class Bone_Hit : MonoBehaviour
{
    private string EventPath = "event:/Items/Bone/boneHit";

    //void Start()
    //{

    //}
    void PlayBoneHitEvent()
    {


        EventInstance BoneHit = RuntimeManager.CreateInstance(EventPath);
        RuntimeManager.AttachInstanceToGameObject(BoneHit, transform, GetComponent<Rigidbody>());

        //Debug.Log("Monster Footstep activated");


        BoneHit.start();
        BoneHit.release();
    }

}
