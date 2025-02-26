using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CameraController : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Cameraholder;
    public Vector3 offset;

    

    public override void OnStartAuthority()
    {
        Cameraholder.SetActive(true);
    }


    public void Update()
    {

        if(SceneManager.GetActiveScene().name == "Game")
        {

           // Cameraholder.transform.position = transform.position + offset;
        
         }
    }

}
