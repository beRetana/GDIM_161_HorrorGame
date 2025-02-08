using MessengerSystem;
using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    void Start()
    {
        DataMessenger.SetGameObject("PlayerDummy", gameObject);
    }
}
