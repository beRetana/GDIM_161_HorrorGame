using MessengerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    private static int _staticClassID = 1;
    private int _myID;

    public delegate void PlayerSpawn(PlayerBase player);
    public static event PlayerSpawn OnPlayerSpawn;

    private void Awake()
    {

        if(_staticClassID > 4)
        {
            Debug.LogError($"Invalid Player Amount: {_staticClassID}");
            Destroy(this.gameObject);
            return;
        }
        _myID = _staticClassID;
        ++_staticClassID;
        Debug.Log($"Player {_myID} spawned");

        OnPlayerSpawn?.Invoke(this);
    }


    public int ID() { return _myID; }
}
