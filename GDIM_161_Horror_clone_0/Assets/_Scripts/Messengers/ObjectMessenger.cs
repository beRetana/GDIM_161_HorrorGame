using System.Collections.Generic;
using UnityEngine;

public class ObjectMessenger : MonoBehaviour
{
    private static Dictionary<string, GameObject> _gameObjectsDictionary;
    private static Dictionary<string, ScriptableObject> _scriptableObjectsDictionary;

    private void Awake()
    {
        _gameObjectsDictionary = new Dictionary<string, GameObject>();
        _scriptableObjectsDictionary = new Dictionary<string, ScriptableObject>();
    }

    public static GameObject GetGameObject(string key)
    {
        if (_gameObjectsDictionary.TryGetValue(key, out GameObject obj))
        {
            return obj;
        }
        else
        {
            return null;
        }
    }

    public static void SetGameObject(string key, GameObject obj)
    {
        if (_gameObjectsDictionary.TryGetValue(key, out _))
        {
            _gameObjectsDictionary[key] = obj;
        }
        else
        {
            _gameObjectsDictionary.Add(key, obj);
        }
    }

    public static ScriptableObject GetScriptableObject(string key)
    {
        if (_scriptableObjectsDictionary.TryGetValue(key, out ScriptableObject obj))
        {
            return obj;
        }
        else
        {
            return null;
        }
    }
    
    public static void SetScriptableObject(string key, ScriptableObject obj)
    {
        if (_scriptableObjectsDictionary.TryGetValue(key, out _))
        {
            _scriptableObjectsDictionary[key] = obj;
        }
        else
        {
            _scriptableObjectsDictionary.Add(key, obj);
        }
    }
}
