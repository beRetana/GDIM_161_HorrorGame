using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Model : MonoBehaviour
{
    public string Door_L = "Door_L_Left_Left_0";
    public string Door_R = "Door_R_Right_Right_0";

    public float speed = 1;

    private Vector3 d0openPos;
    private Vector3 d0closedPos;
    private Vector3 d1openPos;
    private Vector3 d1closedPos;

    private Collider m_Collider;
    private Vector3 m_Size;

    private GameObject myRoot;
    private GameObject door0Object;
    private GameObject door1Object;

    IEnumerator MoveTo(GameObject go, Vector3 destPos)
    {
        float t = 0;
        float rate = speed;

        while (t < 1f && go.transform.position != destPos)
        {
            yield return null;
            t += Time.deltaTime * rate;
            go.transform.position = Vector3.Lerp(go.transform.position, destPos, t);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        
        myRoot = transform.root.gameObject;

        door0Object = GameObject.Find(myRoot.name + "/" + Door_L);
        door1Object = GameObject.Find(myRoot.name + "/" + Door_R);


        m_Collider = GetComponent<Collider>();

        
        m_Size = m_Collider.bounds.size;

        
        d0closedPos = door0Object.transform.position;
        d0openPos = d0closedPos + (transform.forward * (m_Size.z / 2));
        d1closedPos = door1Object.transform.position;
        d1openPos = d1closedPos - (transform.forward * (m_Size.z / 2));

    }

    
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //
        StartCoroutine(MoveTo(door0Object, d0openPos));
        StartCoroutine(MoveTo(door1Object, d1openPos));
    }

    private void OnTriggerExit(Collider other)
    {
        //
        StartCoroutine(MoveTo(door0Object, d0closedPos));
        StartCoroutine(MoveTo(door1Object, d1closedPos));
    }
}
