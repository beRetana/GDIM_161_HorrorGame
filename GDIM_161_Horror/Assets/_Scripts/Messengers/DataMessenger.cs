using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessengerSystem
{
    /// <summary>
    /// Allows for communication of data types and objects; aids decoupling.
    /// </summary>
    public class DataMessenger : MonoBehaviour
    {
        private static Dictionary<string, bool> _boolsDictionary;
        private static Dictionary<string, float> _floatsDictionary;
        private static Dictionary<string, int> _intsDictionary;
        private static Dictionary<string, string> _stringsDictionary;
        private static Dictionary<string, List<string>> _stringListsDictionary;
        private static Dictionary<string, Vector3> _vector3sDictionary;
        private static Dictionary<string, Quaternion> _quaternionsDictionary;
        private static Dictionary<string, GameObject> _gameObjectsDictionary;
        private static Dictionary<string, ScriptableObject> _scriptableObjectsDictionary;

        private static readonly string _DEFAULT_STRING = string.Empty;
        private static readonly List<string> _DEFAULT_STRING_LIST = new();
        private static readonly Vector3 _DEFAULT_VECTOR = Vector3.zero;
        private static readonly Quaternion _DEFAULT_QUATERNION = Quaternion.identity;

        private void Awake()
        {
            _boolsDictionary = new Dictionary<string, bool>();
            _floatsDictionary = new Dictionary<string, float>();
            _intsDictionary = new Dictionary<string, int>();
            _stringsDictionary = new Dictionary<string, string>();
            _stringListsDictionary = new Dictionary<string, List<string>>();
            _vector3sDictionary = new Dictionary<string, Vector3>();
            _quaternionsDictionary = new Dictionary<string, Quaternion>();
            _gameObjectsDictionary = new Dictionary<string, GameObject>();
            _scriptableObjectsDictionary = new Dictionary<string, ScriptableObject>();
        }

        #region Bool

        // String-Key Based Methods
        private static bool GetBool(string key)
        {
            if (!_boolsDictionary.TryGetValue(key, out bool v))
            {
                _boolsDictionary[key] = default;
                return _boolsDictionary[key];
            }
            return v;
        }

        private static void SetBool(string key, bool value)
        {
            _boolsDictionary[key] = value;
        }

        private static void ToggleBool(string key)
        {
            _boolsDictionary[key] = !_boolsDictionary[key];
        }

        private static IEnumerator WaitForBool(string key, bool doInvert = false)
        {
            while (doInvert ? !GetBool(key) : GetBool(key)) yield return null;
        }

        // Enum-Key Based Methods
        public static bool GetBool(MessengerKeys.BoolKey key)
        {
            return GetBool(key.ToString());
        }

        public static void SetBool(MessengerKeys.BoolKey key, bool value)
        {
            SetBool(key.ToString(), value);
        }
        
        public static void ToggleBool(MessengerKeys.BoolKey key)
        {
            ToggleBool(key.ToString());
        }

        /// <summary>
        /// Waits until the bool "key" is false or true if inversed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="doInvert"></param>
        /// <returns> null </returns>
        public static IEnumerator WaitForBool(MessengerKeys.BoolKey key, bool doInvert = false)
        {
            yield return WaitForBool(key.ToString(), doInvert);
        }

        #endregion Bool

        #region Float

        // String-key Based Methods
        private static float GetFloat(string key)
        {
            if (!_floatsDictionary.TryGetValue(key, out float v))
            {
                _floatsDictionary[key] = default;
                return _floatsDictionary[key];
            }
            return v;
        }

        private static void SetFloat(string key, float value)
        {
            _floatsDictionary[key] = value;
        }

        // Enum-Key Based Methods
        public static float GetFloat(MessengerKeys.FloatKey key)
        {
            return GetFloat(key.ToString());
        }
        
        public static void SetFloat(MessengerKeys.FloatKey key, float value)
        {
            SetFloat(key.ToString(), value);
        }

        #endregion Float

        #region GameObject

        // String-Key Based Methods
        private static GameObject GetGameObject(string key)
        {
            if (!_gameObjectsDictionary.TryGetValue(key, out GameObject obj))
            {
                _gameObjectsDictionary[key] = default;
                return _gameObjectsDictionary[key];
            }
            return obj;
        }
        
        private static void SetGameObject(string key, GameObject obj)
        {
            _gameObjectsDictionary[key] = obj;
        }

        // Enum-Key Based Methods
        public static GameObject GetGameObject(MessengerKeys.GameObjectKey key)
        {
            return GetGameObject(key.ToString());
        }

        public static void SetGameObject(MessengerKeys.GameObjectKey key, GameObject obj)
        {
            SetGameObject(key.ToString(), obj);
        }

        #endregion GameObject

        #region Int

        // String-Key Based Methods
        private static int GetInt(string key)
        {
            if (!_intsDictionary.TryGetValue(key, out int v))
            {
                _intsDictionary[key] = default;
                return _intsDictionary[key];
            }
            return v;
        }

        private static void SetInt(string key, int value)
        {
            _intsDictionary[key] = value;
        }

        // Enum-Key Based Methods
        public static int GetInt(MessengerKeys.IntKey key)
        {
            return GetInt(key.ToString());
        }

        public static void SetInt(MessengerKeys.IntKey key, int value)
        {
            SetInt(key.ToString(), value);
        }
        
        #endregion Int

        #region Quaternion

        // String-Key Based Methods
        private static Quaternion GetQuaternion(string key)
        {
            if (!_quaternionsDictionary.TryGetValue(key, out Quaternion v))
            {
                _quaternionsDictionary[key] = _DEFAULT_QUATERNION;
                return _quaternionsDictionary[key];
            }
            return v;
        }

        private static void SetQuaternion(string key, Quaternion value)
        {
            _quaternionsDictionary[key] = value;
        }

        // Enum-Key Based Methods
        public static Quaternion GetQuaternion(MessengerKeys.QuaternionKey key)
        {
            return GetQuaternion(key.ToString());
        }

        public static void SetQuaternion(MessengerKeys.QuaternionKey key, Quaternion value)
        {
            SetQuaternion(key.ToString(), value);
        }

        #endregion Quaternion

        #region ScriptableObject

        // String-Key Based Methods
        private static ScriptableObject GetScriptableObject(string key)
        {
            if (!_scriptableObjectsDictionary.TryGetValue(key, out ScriptableObject obj))
            {
                _scriptableObjectsDictionary[key] = default;
                return _scriptableObjectsDictionary[key];
            }
            return obj;
        }

        private static void SetScriptableObject(string key, ScriptableObject obj)
        {
            _scriptableObjectsDictionary[key] = obj;
        }

        // Enum-Key Based Methods
        public static ScriptableObject GetScriptableObject(MessengerKeys.ScriptableObjectKey key)
        {
            return GetScriptableObject(key.ToString());
        }
        
        public static void SetScriptableObject(MessengerKeys.ScriptableObjectKey key, ScriptableObject obj)
        {
            SetScriptableObject(key.ToString(), obj);
        }

        #endregion ScriptableObject

        #region String

        // String-Key Based Methods
        private static string GetString(string key)
        {
            if (!_stringsDictionary.TryGetValue(key, out string v))
            {
                _stringsDictionary[key] = _DEFAULT_STRING;
                return _stringsDictionary[key];
            }
            return v;
        }
        
        private static void SetString(string key, string value)
        {
            _stringsDictionary[key] = value;
        }

        // Enum-Key Based Methods
        public static string GetString(MessengerKeys.StringKey key)
        {
            return GetString(key.ToString());
        }

        public static void SetString(MessengerKeys.StringKey key, string value)
        {
            SetString(key.ToString(), value);
        }

        #endregion String

        #region StringList

        // String-Key Based Methods
        private static List<string> GetStringList(string key)
        {
            if (!_stringListsDictionary.TryGetValue(key, out List<string> v))
            {
                _stringListsDictionary[key] = _DEFAULT_STRING_LIST;
                return _stringListsDictionary[key];
            }
            return v;
        }

        private static void SetStringList(string key, List<string> value)
        {
            _stringListsDictionary[key] = value;
        }

        private static void AddStringToList(string key, string value)
        {
            List<string> list;
            if (!_stringListsDictionary.TryGetValue(key, out list))
            {
                _stringListsDictionary[key] = _DEFAULT_STRING_LIST;
            }
            list.Add(value);
        }

        public static bool RemoveStringFromList(string key, string value)
        {
            List<string> list;
            if (!_stringListsDictionary.TryGetValue(key, out list))
            {
                return false;
            }
            list.Remove(value);
            return true;
        }

        // Enum-Key Based Methods
        public static List<string> GetStringList(MessengerKeys.StringListKey key)
        {
            return GetStringList(key.ToString());
        }
        
        public static void SetStringList(MessengerKeys.StringListKey key, List<string> value)
        {
            SetStringList(key.ToString(), value);
        }
        
        public static void AddStringToList(MessengerKeys.StringListKey key, string value)
        {
            AddStringToList(key.ToString(), value);
        }
        
        /// <returns>Whether the string was removed.</returns>
        public static bool RemoveStringFromList(MessengerKeys.StringKey key, string value)
        {
            return RemoveStringFromList(key.ToString(), value);
        }

        #endregion StringList

        #region Vector3

        // String-Key Based Methods
        private static Vector3 GetVector3(string key)
        {
            if (!_vector3sDictionary.TryGetValue(key, out Vector3 v))
            {
                _vector3sDictionary[key] = _DEFAULT_VECTOR;
                return _vector3sDictionary[key];
            }
            return v;
        }

        public static void SetVector3(string key, Vector3 value)
        {
            _vector3sDictionary[key] = value;
        }

        // Enum-Key Based Methods
        public static Vector3 GetVector3(MessengerKeys.Vector3Key key)
        {
            return GetVector3(key.ToString());
        }

        public static void SetVector3(MessengerKeys.Vector3Key key, Vector3 value)
        {
            SetVector3(key.ToString(), value);
        }

        #endregion Vector3
    }
}


