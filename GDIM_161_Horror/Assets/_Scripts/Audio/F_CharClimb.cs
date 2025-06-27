using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class F_CharClimb : MonoBehaviour
{
    private string EventPath = "event:/Character/ladderClimb";
  
    //void Start()
    //{
        
    //}
    void PlayClimbEvent()
    {
        
       
        EventInstance Climb = RuntimeManager.CreateInstance(EventPath);
        RuntimeManager.AttachInstanceToGameObject(Climb, transform, GetComponent<Rigidbody>());

       
      

        Climb.start();
        Climb.release();
    }

}
