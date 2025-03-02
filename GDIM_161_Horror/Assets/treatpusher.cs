using UnityEngine;
using System.Collections;

public class treatpusher : MonoBehaviour
{
    private food foodstuff;
    ///public float x,y,z;
    private boxclimber boxclimberstuff;
    private Boxclimberleftdoor boxclimberleftdoorstuff;
    private Boxclimberrightdoor boxclimberrightdoorstuff;
    private Vector3 targetScale2;
    public Vector3 targetScale;
    public Vector3 Tragetscale
    {
        get {return targetScale;}
        set {targetScale = value;}
    }
    private Vector3 initialScale;
    private float scaleSpeed = 2f;
    private bool isScaling = false;
    private bool ismeathandlerup = false;
    //private bool ismeathandlerdown = false;
    public bool Ismeathandlerup
    {
        get { return ismeathandlerup; }  // Returns the value
        set { ismeathandlerup = value; } // Sets the value
    }
     private bool ismeathandlerdown = true;
    public bool Ismeathandlerdown
    {
        get { return ismeathandlerdown; }  // Returns the value
        set { ismeathandlerdown = value; } // Sets the value
    }



   // [SerializeField] private Vector3 targetScale = new Vector3(2f, 2f, 2f); // Target size
    //[SerializeField] private float duration = 2f; // Time in seconds
   // private bool isScaling = false;
    //public bool istreatgoingup = false;
    //float Vector3 wasup = new Vector3(0, 0, 5);
    private Vector3 wasup;
    
void Start()
    {
        foodstuff = FindObjectOfType<food>();
        initialScale = transform.localScale;
        targetScale = new Vector3(transform.localScale.x, transform.localScale.y, 50f);
        targetScale2 = new Vector3( transform.localScale.x, transform.localScale.y, transform.localScale.z);// transform.localScale.z);
        //wasup = new Vector3(transform.localScale.x, transform.localScale.y, 50);
        //_player = FindObjectOfType<FirstPersonController>();
        boxclimberleftdoorstuff=FindObjectOfType<Boxclimberleftdoor>();//jnjnjnjn
        boxclimberrightdoorstuff=FindObjectOfType<Boxclimberrightdoor>();
        boxclimberstuff = FindObjectOfType<boxclimber>();
        //eventstuff = GetComponent<Pavlovevent>();
    }
    void Update()
    {
        
        StartCoroutine(smoothScale(targetScale));
        StartCoroutine(smoothScale2(targetScale2));

        
        
    }
    private IEnumerator smoothScale(Vector3 newScale)
    //private void goup()
    {
        if(boxclimberstuff.Isboxout&&!foodstuff.Isfoodeaten&& boxclimberleftdoorstuff.Leftdooropend && boxclimberrightdoorstuff.Rightdooropend)/////////////
        {
            yield return new WaitForSeconds(1f);
       
        isScaling = true;
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        while (elapsedTime < 1f)
        {
            transform.localScale = Vector3.Lerp(startScale,newScale, elapsedTime);
            
            elapsedTime += Time.deltaTime * scaleSpeed;
            ismeathandlerup = true;
            //Ismeathandlerdown = false;
            yield return null;
        }
        //transform.localScale = wasup;
    }
    }
    private IEnumerator smoothScale2(Vector3 newScale)
    //private void goup()
    {
        
        if (foodstuff.Isfoodeaten)
        {
            Debug.Log("Happy babnjjbja");
        yield return new WaitForSeconds(1f);
        isScaling = true;
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        while (elapsedTime < 1f)
        {
            transform.localScale = Vector3.Lerp(startScale,newScale, elapsedTime);
            
            elapsedTime += Time.deltaTime * scaleSpeed;
            ismeathandlerdown = true;
           //ismeathandlerup = false;// watch for this
            yield return null;
        }
        //transform.localScale = wasup;
        }
    }
    
    

        //transform.localScale = newScale; // Ensure exact final scale
       // isScaling = false;
        //istreatgoingup = true;
    }
