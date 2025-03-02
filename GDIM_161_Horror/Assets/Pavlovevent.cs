using UnityEngine;
using StarterAssets;
using System.Collections;
public class Pavlovevent : MonoBehaviour
{
    private bool eventishit = false;
    public bool Eventishit
    {
       get {return eventishit;}
       set { eventishit = value;}
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
         
          eventishit = true;
        }
    }
    
    
    

    
    
}
