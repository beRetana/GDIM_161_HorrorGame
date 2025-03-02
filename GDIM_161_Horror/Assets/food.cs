using UnityEngine;
using System.Collections;
public class food : MonoBehaviour
{
    private Pavlovevent eventstuff;
    private Pavlovevent pavloveventhit;
    private boxclimber boxclimberstuff;
    private bool isfoodeaten = false;
    
    private treatpusher treatpusherstuff;
    private Vector3 targetScale2;
    private Vector3 targetScale;
    private Vector3 foodclimb;
    private Vector3 foodback;
    private float scaleSpeed = 2f;
    [SerializeField] private float moveDuration = 1f; // Adjust for speed
    [SerializeField] private float moveDuration2 = 2f; // Adjust for speed
    bool isScaling;

    public bool Isfoodeaten
    {
        get { return isfoodeaten; }  // Returns the value
         set {  isfoodeaten = value; } // Sets the value // ask about priavte 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isfoodeaten)
        {
            Debug.Log("happy brother");
            isfoodeaten = true;
           // eventstuff.Eventishit= false;

        //GetComponent<Renderer>().enabled = false; // Hide the object visually
       // GetComponent<Collider>().enabled = false;// Destroy the food object

        }
    }
    void Start()
    {
       // foodclimb =new Vector3(transform.position.x,transform.position.y+2,transform.position.z);
       // foodback =new Vector3(transform.position.x,transform.position.y,transform.position.z);
       // boxclimberstuff = FindObjectOfType<boxclimber>();
       // treatpusherstuff= FindObjectOfType<treatpusher>();
      //  eventstuff= FindObjectOfType<Pavlovevent>();
       // targetScale= new Vector3( transform.position.x,transform.position.y,transform.position.z+50);
        //targetScale2= new Vector3 ( transform.position.x,transform.position.y,transform.position.z);
    
    }

    // Update is called once per frame
    void Update()
    {
//foodup();    
//fooddown();
    }
    private void foodup()
    {
if(treatpusherstuff.Ismeathandlerup)
{
    StartCoroutine(SmoothMoveToPosition());
}
    }

    private void fooddown()
    {
        if (!treatpusherstuff.Ismeathandlerup)
        {
            StartCoroutine(SmoothbackToPosition());
        }
    }
    private IEnumerator SmoothMoveToPosition()
    {
        
      //  if (snapPosition == null)
      //  {
        //    Debug.LogError("snapPosition is not assigned in Inspector!");
         //   yield break; // Stop execution
      //  }    
           
        Vector3 startPosition = transform.position;
        Vector3 endposition = transform.position+foodclimb;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            //_player.Headbop = false;
            //_player.IsClimbing = false;
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, foodclimb, t);
            
            //transform.rotation = Quaternion.Slerp(startRotation, transform.rotation, t);
            yield return null;
             
        }
           }
           private IEnumerator SmoothbackToPosition()
    {
        
        yield return new WaitForSeconds(2f);
        Vector3 startPosition2 = transform.position;
        Quaternion startRotation2 = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration2)
        {
            //_player.Headbop = false;
            //_player.IsClimbing = false;
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration2;
            transform.position = Vector3.Lerp(startPosition2, foodback, t);
            //transform.rotation = Quaternion.Slerp(startRotation2, snapPosition2.rotation, t);
            //foodstuff.Isfoodeaten= false;
            yield return null;
        }
    //foodgodown()
    //{

    //}
    
}
}
