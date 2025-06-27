using FMODUnity;
using Unity.VisualScripting;
using System.Collections;
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
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    PlayASoundAtALocation();
        //}
    }
    private IEnumerator RandomSoundTimer()
    {
        while (true)
        {
            float delay = Random.Range(180f, 300f); // 180–300 seconds (3–5 minutes)
            yield return new WaitForSeconds(delay);
            PlayASoundAtALocation();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(RandomSoundTimer());
    }

    private void OnDisable()
    {
        StopCoroutine(RandomSoundTimer());
    }



    private void PlayASoundAtALocation()
    {
        //float randomNum = Random.Range(0f, 1f);
        //Vector3 randomLocation;
        //randomLocation = location1.transform.position;



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
