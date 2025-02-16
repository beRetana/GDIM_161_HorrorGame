using MessengerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class PlayerBase : MonoBehaviour
{
    private static int _staticClassID = 1;
    private int _myID;

    public delegate void PlayerSpawn(PlayerBase player);
    public static event PlayerSpawn OnPlayerSpawn;

   

    PlayerState playerController;
    //Animator anim;
    PlayerState currentState;

    private void Awake()
    {
        AssignID();
    }

    private void Start()
    {
        currentState = new Locked(this.gameObject);
    }

    private void Update()
    {
        currentState = currentState.Process();
    }

    private bool AssignID()
    {
        if (_staticClassID > 4)
        {
            Debug.LogError($"Invalid Player Amount: {_staticClassID}");
            Destroy(this.gameObject);
            return false;
        }
        _myID = _staticClassID;
        ++_staticClassID;
        Debug.Log($"Player {_myID} spawned");

        OnPlayerSpawn?.Invoke(this);

        return true;
    }


    public int ID() { return _myID; }
}
