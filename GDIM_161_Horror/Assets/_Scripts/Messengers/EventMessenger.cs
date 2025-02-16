using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace MessengerSystem
{
    /// <summary>
    /// Allows for communication of Events; aids decoupling.
    /// </summary>
    public class EventMessenger : MonoBehaviour
    {
        private Dictionary<MessengerKeys.EventKey, UnityEvent> _eventDictionary;

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
                _eventDictionary = new Dictionary<MessengerKeys.EventKey, UnityEvent>();
            }
        }

        /// <summary>
        /// Make listener subscribe to the event binded to the eventKey. If not event is binded to this key
        /// it creates a new UnityEvent, binds it to the eventKey and makes listener subscribe to it.
        /// </summary>
        /// <param name="eventKey">A Messenger.EventKey type that is binded to the UnityEvent.</param>
        /// <param name="listener">A UnityAction type (event) that gets subscribed to the UnityEvent.</param>
        public static void StartListeningTo(MessengerKeys.EventKey eventKey, UnityAction listener)
        {
            UnityEvent thisEvent;
            if (Instance._eventDictionary.TryGetValue(eventKey, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                Instance._eventDictionary.Add(eventKey, thisEvent);
            }
        }

        /// <summary>
        /// Make listener unsubscribe to the event binded to the eventKey.
        /// </summary>
        /// <param name="eventKey">A Messenger.EventKey type that is binded to the UnityEvent.</param>
        /// <param name="listener">A UnityAction type (event) that gets unsubscribed to the UnityEvent.</param>
        public static void StopListeningTo(MessengerKeys.EventKey eventKey, UnityAction listener)
        {
            if (_instance == null) return;
            UnityEvent thisEvent;
            if (Instance._eventDictionary.TryGetValue(eventKey, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        /// <summary>
        /// Invokes the UnityEvent binded to the eventKey.
        /// </summary>
        /// <param name="eventKey">A Messenger.EventKey type that is binded to the UnityEvent.</param>
        public static void TriggerEvent(MessengerKeys.EventKey eventKey)
        {
            UnityEvent thisEvent;
            if (Instance._eventDictionary.TryGetValue(eventKey, out thisEvent))
            {
                thisEvent.Invoke();
            }
            else
            {
                Debug.Log("EventMessenger doesn't contain the event: " + eventKey);
            }
        }
    }
}
