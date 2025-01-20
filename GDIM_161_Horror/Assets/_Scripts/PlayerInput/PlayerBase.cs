using MessengerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    private static int _staticClassID = 1;
    private int _myID;

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
        PlayerMessengerKey();
    }

    void PlayerMessengerKey()
    {
        switch (_myID)
        {
            case 1:
                DataMessenger.SetGameObject(MessengerKeys.GameObjectKey.Player1, gameObject);
                break;
            case 2:
                DataMessenger.SetGameObject(MessengerKeys.GameObjectKey.Player2, gameObject);
                break;
            case 3:
                DataMessenger.SetGameObject(MessengerKeys.GameObjectKey.Player3, gameObject);
                break;
            case 4:
                DataMessenger.SetGameObject(MessengerKeys.GameObjectKey.Player4, gameObject);
                break;
            default:
                Debug.LogError($"Invalid Player Amount: {_myID}");
                break;
        }
    }
}
