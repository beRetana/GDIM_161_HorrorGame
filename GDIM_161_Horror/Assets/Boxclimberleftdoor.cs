using UnityEngine;
using StarterAssets;
using System.Collections;
public class Boxclimberleftdoor : MonoBehaviour
{
    [SerializeField] private Transform snapPosition;
    [SerializeField] private Transform snapPosition2;
    
    private food foodstuff;
   // private FirstPersonController _player;
   private boxclimber boxclimberstuff;
   // private bool _playerInRange = false;
    private float moveDuration = 0.2f; // Adjust for speed
    //public bool isboxout = false;
    private treatpusher treatpusherstuff;
    private bool leftdooropend = false;
    public bool Leftdooropend
    {
        get {return leftdooropend;}
        set { leftdooropend = value;}
    }
    private bool leftdoorclosed = true;

    
    public bool Leftdoorclosed
    {
       get { return leftdoorclosed; }  // Returns the value
       set { leftdoorclosed = value; } 
    }
     /*private bool leftdooropen = false;
     public bool Leftdooropen
     {
        get {return leftdooropen }
        set { leftdooropen=value;}
     }*/
    
    void Start()
    {
        foodstuff = FindObjectOfType<food>();
        treatpusherstuff = FindObjectOfType<treatpusher>();
        //_player = FindObjectOfType<FirstPersonController>();
        boxclimberstuff = FindObjectOfType<boxclimber>();
        //eventstuff = GetComponent<Pavlovevent>();
    }
    // fixed
    void Update()
    {
      moveleftdoor();
      closeleftdoor();
     
    }
     
    private void moveleftdoor()
    {
       if(boxclimberstuff.Isboxout && leftdoorclosed && !leftdooropend &&!foodstuff.Isfoodeaten ) // keeps looping ghassan
       {
            StartCoroutine(SmoothMoveToPosition());
            leftdooropend = true;
            leftdoorclosed = false;
           // boxclimberstuff.Pavlovclosed = false;// carful
           // Debug.Log(eventstuff.isdooropen);

        }
    }
    private void closeleftdoor()
    {
        if ( treatpusherstuff.Ismeathandlerdown && !leftdoorclosed && leftdooropend && foodstuff.Isfoodeaten)/// open
        {
            //Debug.Log(" who dosent love superman");
            StartCoroutine(SmoothcloseToPosition());
            leftdoorclosed = true;
            leftdooropend = false;
            //boxclimberstuff.Isboxout = false;
            //boxclimberstuff.Pavlovclosed = true; // carful
        }
    }

    private IEnumerator SmoothMoveToPosition()
    {
        if (snapPosition == null)
        {
            Debug.LogError("snapPosition is not assigned in Inspector!");
            yield break; // Stop execution
        }
         yield return new WaitForSeconds(1f);
        Vector3 startPosition = transform.position;
        Quaternion startRotation =transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            //_player.Headbop = false;
            //_player.IsClimbing = false;
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, snapPosition.position, t);
            transform.rotation = Quaternion.Slerp(startRotation, snapPosition.rotation, t);
            boxclimberstuff.Isdooropen = true;
             
             
            yield return null;
            boxclimberstuff.Isboxout = true; 
        }
        
        //Isfoodeaten
    }
    private IEnumerator SmoothcloseToPosition()
    {
        if (snapPosition == null)
        {
            Debug.LogError("snapPosition is not assigned in Inspector!");
            yield break; // Stop execution
        }

         yield return new WaitForSeconds(1f);
        Vector3 startPosition = transform.position;
        Quaternion startRotation =transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            //_player.Headbop = false;
            //_player.IsClimbing = false;
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, snapPosition2.position, t);
            transform.rotation = Quaternion.Slerp(startRotation, snapPosition2.rotation, t);
            //eventstuff.Isdooropen = true;
            
             
             
            yield return null;
            boxclimberstuff.Isboxout = false; 
        }
        
        //Isfoodeaten
    }
    
}