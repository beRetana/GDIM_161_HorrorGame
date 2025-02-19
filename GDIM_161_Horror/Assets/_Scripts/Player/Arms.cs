using UnityEngine;
using System.Collections;
using System;

namespace Player
{
    public class Arms : MonoBehaviour
    {
        [Header("Shoulders")]
        [SerializeField] float shoulderRotationSpeed;
        [SerializeField] float shoulderTopClamp;
        [SerializeField] float shoulderBottomClamp;
        [Tooltip("Relative to camera rotation amount")]
        [SerializeField] float shoulderRotateScalar;
        [Tooltip("Relative to camera rotation speed")]
        [SerializeField] float shoulderRotateSpeed;

        [Header("Arms")]
        [SerializeField] Transform LArm;
        [SerializeField] Transform RArm;

        [Header("Interact Animation")]
        [Tooltip("Distance to Move")]
        [SerializeField] float amplitude = .4f;
        [Tooltip("Animation Speed")]
        [SerializeField] float frequency = 2f;
        [SerializeField] float duration = 1f;

        private bool inOutStretchHandAnimation;

        private void Start()
        {
            SetHandDominancePosition(false, true);
        }

        public void SetHandDominancePosition(bool isLeft, bool isRight)
        {
            LArm.localRotation = Quaternion.Euler(isLeft ? 0f : 10f, 0f, 0f);
            RArm.localRotation = Quaternion.Euler(isRight ? 0f : 10f, 0f, 0f);
        }

        public void OutStretchHand(bool isLHandDom)
        {
            if (inOutStretchHandAnimation) return;
            inOutStretchHandAnimation = true;
            StartCoroutine(HandAnimation(isLHandDom ? LArm : RArm));
        }

        private IEnumerator HandAnimation(Transform handTransform)
        {
            Vector3 startPosition = handTransform.localPosition;
            float time = 0f;
            float elapsedTime = 0f;

            while(elapsedTime < duration)
            {
                time += Time.deltaTime * frequency;
                float offset = Mathf.Sin(time) * amplitude;
                handTransform.localPosition = startPosition + new Vector3(0f, 0f, offset);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            handTransform.localPosition = startPosition;
            inOutStretchHandAnimation = false;
        }

        /*public void ArmsRotation(float lookAmount, bool isCurrentDeviceMouse)
        {
            Vector3 currentRotation = transform.localEulerAngles;

            float pitch = currentRotation.x;
            if (pitch > 180f) pitch -= 360f;

            pitch += lookAmount * shoulderRotationSpeed * (isCurrentDeviceMouse ? 1.0f : Time.deltaTime);
            pitch = Mathf.Clamp(pitch, shoulderBottomClamp, shoulderTopClamp);

            transform.localRotation = Quaternion.Euler(pitch, currentRotation.y, currentRotation.z);
        }*/

        public void ArmsRotation(float rotationVelocity, float cinemachineTargetPitch)
        {
            transform.localRotation = Quaternion.Euler(cinemachineTargetPitch * shoulderRotateScalar, 0f, 0f);
            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }
}
