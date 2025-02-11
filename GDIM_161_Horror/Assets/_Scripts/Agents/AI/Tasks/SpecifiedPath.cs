using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI{

    public class SpecifiedPath : TaskBase{

        [Header("Specified Path Settings")]
        [SerializeField] private List<string> roomName;
        [SerializeField] private bool specifiedPath;
        [SerializeField] private float pathStopWaitTime;
        [SerializeField] private ChoosingType choosingType;
        
        [Serializable]
        private enum ChoosingType{
            Random,
            Specific
        }
        
        private List<Transform> path = new List<Transform>();
        private RoomPathSetter roomPathSetter;
        private int indexOfPath = 0;

        public void SetNames(List<string> newNames){
            roomName = newNames;
        }  
    }
}

