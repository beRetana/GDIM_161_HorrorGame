using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    [SerializeField] private Mode mode;

    private enum Mode{
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted,
    }

    private void LateUpdate(){
        switch (mode){
            case Mode.LookAtInverted: 
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAt:
                Vector3 cameraDirection = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + cameraDirection);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}
