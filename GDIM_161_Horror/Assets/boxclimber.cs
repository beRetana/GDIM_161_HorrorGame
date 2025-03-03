using UnityEngine;
using System.Collections;
public class boxclimber : MonoBehaviour
{
     [SerializeField] private Transform snapPosition;
     public Transform SnapPosition
    {
        get { return snapPosition; }
        set { snapPosition = value; }
    }

    [SerializeField] private Transform snapPosition2;

   // private FirstPersonController _player;
   private boxclimber Boxclimber;
   private Boxclimberrightdoor  rightdoorstuff ;
   private  Boxclimberleftdoor leftdoorstuff ;
   private food foodstuff;
   private Pavlovarea pavlovareastuff;
   private Pavlovevent pavloveventhit;
   private bool pavlovclosed = true;
   public bool Pavlovclosed
   {
    get {return pavlovclosed;}
    set { pavlovclosed = value;}
   }
   // private bool _playerInRange = false;
    [SerializeField] private float moveDuration = 1f; // Adjust for speed
    [SerializeField] private float moveDuration2 = 1f; // Adjust for speed
    private bool isboxout = false;
    public bool Isboxout
    {
        get {return isboxout;}
        set { isboxout=value;}
    }

    private bool didplayerhit = false; // will add logic later
    private bool isdooropen = false;
    //private bool eventishit = false;
    public bool Isdooropen
    {
        get { return isdooropen; }  // Returns the value
        set { isdooropen = value; } // Sets the value
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pavloveventhit=FindObjectOfType<Pavlovevent>();
        foodstuff = FindObjectOfType<food>();
        //_player = FindObjectOfType<FirstPersonController>();
        //Boxclimber = FindObjectOfType<boxclimber>();
        rightdoorstuff = FindObjectOfType<Boxclimberrightdoor>();
        leftdoorstuff =FindObjectOfType<Boxclimberleftdoor>();
        pavlovareastuff = FindObjectOfType<Pavlovarea>();
    }

    // Update is called once per frame
    void Update()
    {
        takeboxup();
      takeboxdown();
    }
    private void takeboxdown()
    {
      if (leftdoorstuff.Leftdoorclosed && rightdoorstuff.Rightdoorclosed && foodstuff.Isfoodeaten)
      {
        Debug.Log(" Im happy bro");
         StartCoroutine(SmoothbackToPosition());
      pavloveventhit.Eventishit = false;
      pavlovareastuff.Ismonsterinarea= false;
      isboxout = false;
      }
      
    }
    private void takeboxup()
    {
        if (pavloveventhit.Eventishit && pavlovareastuff.Ismonsterinarea)
           {
            StartCoroutine(SmoothMoveToPosition());
            isboxout = true;
           }
            
    }
    private IEnumerator SmoothMoveToPosition()
    {
        
        if (snapPosition == null)
        {
            Debug.LogError("snapPosition is not assigned in Inspector!");
            yield break; // Stop execution
        }    
           
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            //_player.Headbop = false;
            //_player.IsClimbing = false;
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, snapPosition.position, t);
            transform.rotation = Quaternion.Slerp(startRotation, snapPosition.rotation, t);
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
            transform.position = Vector3.Lerp(startPosition2, snapPosition2.position, t);
            transform.rotation = Quaternion.Slerp(startRotation2, snapPosition2.rotation, t);
            foodstuff.Isfoodeaten= false;
            yield return null;
        }
            //isboxout = false;
           // isboxout = false; 
        }
    }
