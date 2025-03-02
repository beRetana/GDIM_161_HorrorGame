 /* using UnityEngine;
using StarterAssets;
using System.Collections;
public class FollowScalingObject : MonoBehaviour
{
    //[SerializeField] private Transform target; // Reference to the scaling collider
    //private Vector3 initialOffset; // Store initial offset from target
    //private treatpusher Treatposition;
    void Start()
    {
        //if (target != null)
      //  {
           // initialOffset = transform.position - target.position; // Calculate offset
      //  }
    }

    void Update()
    {
        if (Treatposition.istreatgoingup)if(Input.GetKeyDown(KeyCode.Space)) 
        {
         //   Debug.Log("happy feet");
         transform.position = transform.position + new Vector3(0,0,15 * Time.deltaTime); 
         //new Vector3(Treatposition.x,Treatposition.y,Treatposition.z) + initialOffset;
        }
    }
} */