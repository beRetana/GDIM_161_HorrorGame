using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventMessenger : MonoBehaviour
{
    private Dictionary<string, UnityEvent> _eventDictionary;

    private static EventMessenger _instance;

    public static EventMessenger Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(EventMessenger)) as EventMessenger;

                if (!_instance)
                {
                    Debug.LogError("There should be one EventMessenger on a GameObject in your scene.");
                }
                else
                {
                    _instance.Init();
                }
            }

            return _instance;
        }
    }

    void Init()
    {
        if (_eventDictionary == null)
        {
            _eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListeningTo(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance._eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListeningTo(string eventName, UnityAction listener)
    {
        if (_instance == null) return;
        UnityEvent thisEvent;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
        else
        {
            Debug.Log("EventMessenger doesn't contain the event: " + eventName);
        }
    }
}
