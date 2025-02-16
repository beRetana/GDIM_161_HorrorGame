using System.Collections.Generic;
using UnityEngine;

/*
    More data types can be added to this script if its necessary.
    Also I might the key type of the dictionaries to type enum
    or something else that is better than string (prone to mispelling).
*/

public class DataMessenger : MonoBehaviour
{
    // These Dictionaries are to store data binded to string keys.

    private static Dictionary<string, float> _floatsDictionary;
    private static Dictionary<string, int> _intsDictionary;
    private static Dictionary<string, string> _stringsDictionary;
    private static Dictionary<string, List<string>> _stringListsDictionary;
    private static Dictionary<string, bool> _boolsDictionary;
    private static Dictionary<string, Vector3> _vector3sDictionary;
    private static Dictionary<string, Quaternion> _quaternionsDictionary;

    // These are the default values for data that is not found in the dictionary.
    private static int _defaultIntValue = 0;
    private static float _defaultFloatValue = 0;
    private static bool _defaultBoolValue = false;
    private static string _defaultStringValue = string.Empty;
    private static List<string> _defaultStringListValue = new List<string>();
    private static Vector3 _defaultVector3Value = Vector3.zero;

    private void Awake()
    {
        _floatsDictionary = new Dictionary<string, float>();
        _intsDictionary = new Dictionary<string, int>();
        _stringsDictionary = new Dictionary<string, string>();
        _stringListsDictionary = new Dictionary<string, List<string>>();
        _boolsDictionary = new Dictionary<string, bool>();
        _vector3sDictionary = new Dictionary<string, Vector3>();
        _quaternionsDictionary = new Dictionary<string, Quaternion>();
    }

    /*
        These getter methods return the value of the data that is asked for if 
        it is found in the dictionary. If the key is not found in the dictionary
        then a new element with that key is created, its corresponding
        default value is assigned and returned.
    */
    public static float GetFloat(string key)
    {
        if (!_floatsDictionary.TryGetValue(key, out float v))
        {
            _floatsDictionary[key] = _defaultFloatValue;
            return _floatsDictionary[key];
        }
        return v;
    }

    public static void SetFloat(string key, float value)
    {
        _floatsDictionary[key] = value;
    }

    public static int GetInt(string key)
    {
        if (!_intsDictionary.TryGetValue(key, out int v))
        {
            _intsDictionary[key] = _defaultIntValue;
            return _intsDictionary[key];
        }
        return v;
    }
    
    public static void SetInt(string key, int value)
    {
        _intsDictionary[key] = value;
    }

    public static string GetString(string key)
    {
        if (!_stringsDictionary.TryGetValue(key, out string v))
        {
            _stringsDictionary[key] = _defaultStringValue;
            return _stringsDictionary[key];
        }
        return v;
    }

    public static void SetString(string key, string value)
    {
        _stringsDictionary[key] = value;
    }

    public static List<string> GetStringList(string key)
    {
        if (!_stringListsDictionary.TryGetValue(key, out List<string> v))
        {
            _stringListsDictionary[key] = _defaultStringListValue;
            return _stringListsDictionary[key];
        }
        return v;
    }

    public static void SetStringList(string key, List<string> value)
    {
        _stringListsDictionary[key] = value;
    }

    public static bool GetBool(string key)
    {
        if (!_boolsDictionary.TryGetValue(key, out bool v))
        {
            _boolsDictionary[key] = _defaultBoolValue;
            return _boolsDictionary[key];
        }
        return v;
    }

    public static void SetBool(string key, bool value)
    {
        _boolsDictionary[key] = value;
    }

    public static Vector3 GetVector3(string key)
    {
        if (!_vector3sDictionary.TryGetValue(key, out Vector3 v))
        {
            _vector3sDictionary[key] = _defaultVector3Value;
            return _vector3sDictionary[key];
        }
        return v;
    }

    public static void SetVector3(string key, Vector3 value)
    {
        _vector3sDictionary[key] = value;
    }

    public static Quaternion GetQuaternion(string key)
    {
        if (!_quaternionsDictionary.TryGetValue(key, out Quaternion v))
        {
            _quaternionsDictionary[key] = Quaternion.identity;
            return _quaternionsDictionary[key];
        }
        return v;
    }

    public static void SetQuaternion(string key, Quaternion value)
    {
        _quaternionsDictionary[key] = value;
    }
}
