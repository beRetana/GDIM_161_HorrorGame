using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using StarterAssets;

public class VolumeMenu : MonoBehaviour
{
    
    [Header ("Components")]
    
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject firstSelected;

    [Header ("Player")]
    [SerializeField] private FirstPersonController firstPersonController;



    private void Start()
    {
        menu.gameObject.SetActive(false);

        
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleVolumeMenu();
            
        }
    }


    private void ToggleVolumeMenu()
    {
        //turn menu OFF
        if(menu.gameObject.activeInHierarchy)
        {
            menu.gameObject.SetActive(false);
        }

        else
        {
            menu.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);

        }

    }
}
