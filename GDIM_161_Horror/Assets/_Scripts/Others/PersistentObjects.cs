using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// This script sets the object it is attached to as Do not destroy in
    /// scene load. If there is another object with the tag privided it will 
    /// destroy itself as this object should be the only one with that tag.
    /// </summary>
public class PersistentObject : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
