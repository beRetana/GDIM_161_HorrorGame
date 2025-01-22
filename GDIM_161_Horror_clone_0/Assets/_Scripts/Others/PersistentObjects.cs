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
    [SerializeField] private string _objectsTag;

    void Awake()
    {
        GameObject[] objectsListWithTag = GameObject.FindGameObjectsWithTag(_objectsTag);

        if (objectsListWithTag.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
