using UnityEngine;
using StarterAssets;
using System.Collections;
public class Boxclimberrightdoor : MonoBehaviour

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
    
    private bool rightdooropend = false;
    public bool Rightdooropend
    {
    get{return rightdooropend; }
    set {rightdooropend = value;}
    }
    private bool rightdoorclosed=true;
    public bool Rightdoorclosed
    {
       get { return rightdoorclosed; }  // Returns the value
       set { rightdoorclosed = value; } 
    }
    void Start()
    {
        foodstuff = FindObjectOfType<food>();
        treatpusherstuff = FindObjectOfType<treatpusher>();
        //_player = FindObjectOfType<FirstPersonController>();
        boxclimberstuff = FindObjectOfType<boxclimber>();
        //eventstuff = GetComponent<Pavlovevent>();
    }
    void Update()
    {
      moverightdoor();  
      closerightdoor();
    }

    private void moverightdoor()
    {
       if(boxclimberstuff.Isboxout && rightdoorclosed && !rightdooropend && !foodstuff.Isfoodeaten)
       {
            StartCoroutine(SmoothMoveToPosition());
            rightdooropend = true;
            rightdoorclosed = false;
            
            //boxclimberstuff.Pavlovclosed = false;
            
        }
    }
    private void closerightdoor()
    {
        if ( treatpusherstuff.Ismeathandlerdown && !rightdoorclosed && rightdooropend && foodstuff.Isfoodeaten)
        {
            
            StartCoroutine(SmoothcloseToPosition());
            rightdoorclosed = true;
            rightdooropend = false;
            //eventstuff.isboxout = false;
            //boxclimberstuff.Pavlovclosed = true;
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
            //isboxout = true; 
        }
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
            //isboxout = true; 
        }
}
}