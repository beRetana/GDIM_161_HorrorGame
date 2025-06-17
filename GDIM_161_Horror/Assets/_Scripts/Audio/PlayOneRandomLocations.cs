using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class PlayOneRandomLocations : MonoBehaviour
{

    [SerializeField]
    private GameObject location1;
    //[SerializeField]
    //private GameObject location2;
    //[SerializeField]
    //private GameObject location3;

   // [SerializeField] private EventReference MonsterCall;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayASoundAtALocation();
        }
    }

    private void PlayASoundAtALocation()
    {
        //float randomNum = Random.Range(0f, 1f);
        Vector3 randomLocation;
        randomLocation = location1.transform.position;

        //if (randomNum < 0.33f)
        //{
        //    randomLocation = location1.transform.position;
        //}
        //else if (randomNum < 0.66f)
        //{
        //    randomLocation = location2.transform.position;
        //}
        //else
        //{ 
        //    randomLocation = location3.transform.position;
        //}
        AudioManager.instance.PlayOneShot(FMODEvents.instance.MonsterDeerCall, this.transform.position);
        //RuntimeManager.PlayOneShot(MonsterCall, randomLocation);
        //Debug.Log(randomLocation);
    
    }





}
