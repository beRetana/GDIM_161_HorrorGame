using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class F_DeerFst : MonoBehaviour
{
    private int MaterialType;
    private RaycastHit rh;
    private float distance = 0.3f;
    private string EventPath = "event:/deerFootsteps";
    private PARAMETER_ID ParamID;
    private PARAMETER_ID ParamID2;
    private LayerMask LM;

    
    //Section Used for finding FMOD parameter ID number (instead of name)
    
    // private EventDescription EventDes;
    // private PARAMETER_DESCRIPTION ParamDes;
    

    private void Start()
    {
        // This section returns the parameter ID in the console
        /*
        EventDes = RuntimeManager.GetEventDescription(EventPath);
        EventDes.getParameterDescriptionByName("Terrain", out ParamDes);
        ParamID = ParamDes.id;
        Debug.Log(ParamID.data1 + " " + ParamID.data2);  */
        

        // Assigns the ID of 'Terrain'
            // ParamID.data1 = 1082362436;
            // ParamID.data2 = 3768982689;
        // Assigns the ID for 'WalkRun'
                // ParamID2.data1 = 4001576615;
                // ParamID2.data2 = 3268475985;
        LM = LayerMask.GetMask("Ground");        
    }

    /*
    void Update()
    {
        // Shows drawn raycast for debugging
        Debug.DrawRay(transform.position, Vector3.down * distance, Color.blue);
    }
    */

    void DeerRunEvent()
    {
        // Start with material check then instantiate sound
        MaterialCheck();
        EventInstance Run = RuntimeManager.CreateInstance(EventPath);
        RuntimeManager.AttachInstanceToGameObject(Run, transform, GetComponent<Rigidbody>());

        // Sets the Terrain parameter
            //Run.setParameterByID(ParamID, MaterialValue, false);

        // Sets the 'WalkRun' parameter
            // Run.setParameterByID(ParamID2, 1, false);

        // Can be used as alternative to IDs
        Run.setParameterByName("Terrain", MaterialType);
        Run.setParameterByName("WalkRun", 1, false);

        Run.start();
        Run.release();
    }

    void DeerWalkEvent()
    {
        // Start with material check then instantiate sound
        MaterialCheck();
        EventInstance Walk = RuntimeManager.CreateInstance(EventPath);
        RuntimeManager.AttachInstanceToGameObject(Walk, transform, GetComponent<Rigidbody>());

        // Sets the Terrain parameter
            //Walk.setParameterByID(ParamID, MaterialValue, false);

        // Sets the 'WalkRun' parameter
        // Walk.setParameterByID(ParamID2, 1, false);

        // Can be used as alternative to IDs
        Walk.setParameterByName("Terrain", MaterialType);

        Walk.start();
        Walk.release();
    }

    // Sets parameter based on RaycastHit
    void MaterialCheck()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out rh, distance, LM))
        {
            //Debug.Log(rh.collider.tag + " " + MaterialValue);
            switch (rh.collider.tag)
            {
                case "Grass":
                    MaterialType = 0; // Labeled parameters in FMOD
                    break;
                case "Stone":
                    MaterialType = 1;
                    break;
                case "Metal":
                    MaterialType = 2;
                    break;
                case "Gravel":
                    MaterialType = 3;
                    break;
                case "Wood":
                    MaterialType = 4;
                    break;
                case "Water":
                    MaterialType = 5;
                    break;
                default:
                    MaterialType = 0;
                    break;

            }
        }
    }
}