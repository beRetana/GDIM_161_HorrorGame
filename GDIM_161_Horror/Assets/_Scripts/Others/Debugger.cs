using System;
using UnityEngine;
using System.Collections.Generic;

namespace OtherUtils
{
    public class Debugger : MonoBehaviour
    {
        [Serializable]
        struct Pair
        {
            public MonoBehaviour debugScript;
            public bool enabled;
        }

        [SerializeField, Tooltip("MUST Implement IDebugger")] private List<Pair> debuggers;

        private void Start()
        {
            foreach (Pair pair in debuggers)
                (pair.debugScript as IDebugger)?.SetDebugActive(pair.enabled);

        }
    }
}
