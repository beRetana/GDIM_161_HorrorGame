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
        [SerializeField, Tooltip("Relative to camera rotation amount")] float shoulderRotateScalar;
        [SerializeField, Tooltip("Relative to camera rotation speed")] float shoulderRotateSpeed;

        [Header("Arms")]
        [SerializeField] Transform LArm;
        [SerializeField] Transform RArm;

        [Header("Interact Animation")]
        [SerializeField, Tooltip("Distance to Move")] float amplitude = .4f;
        [SerializeField, Tooltip("Animation Speed")] float frequency = 2f;
        [SerializeField, Tooltip("Sinosudal from In to Out")] float duration = 1f;

        //private bool leftHandDom;

        //private bool inOutStretchHandAnimation;
        //private bool handOutStretched;

        public enum HandStateEnum
        {
            None = 0,                   //000
            IsDom = 1 << 0,             //001
            IsOutStretched = 1 << 1,    //010
            IsInAnimation = 1 << 2      //100
        }

        private class Hand
        {
            private HandStateEnum handState = 000;
            public Vector3 RecentStartPos { get; private set; }
            
            public void SetStartPos(Vector3 position)
            {
                RecentStartPos = position;
            }

            public bool IsDom() { return (handState & HandStateEnum.IsDom) != 0; }
            public bool IsOutStretched() { return (handState & HandStateEnum.IsOutStretched) != 0; }
            public bool IsInAnimation() { return (handState & HandStateEnum.IsInAnimation) != 0; }

            public void SetDom(bool isDom) { HandStateBool(isDom, HandStateEnum.IsDom); }
            public void SetOutStretched(bool isOutStretched) { HandStateBool(isOutStretched, HandStateEnum.IsOutStretched);}
            public void SetInAnimation(bool isInAnimation) { HandStateBool(isInAnimation, HandStateEnum.IsInAnimation); }
            private void HandStateBool(bool isTrue, HandStateEnum flag) { handState = isTrue ? handState | flag : handState & ~flag; }
        }

        private Hand leftHand;
        private Hand rightHand;

        private void Start()
        {
            SetHandDominancePosition(false, true);
        }

        /*private void SetHandStartPos(bool isLeftDom, Vector3 position)
        {
            if (isLeftDom)
                recentLHandStartPos = position;
            else
                recentRHandStartPos = position;
        }*/

        private bool IsLDom()
        {
            return leftHand.IsDom();
        }
        private Hand GetDominantHand()
        {
            return IsLDom() ? leftHand : rightHand;
        }
        private Transform GetDominantHandTransform()
        {
            return IsLDom() ? LArm : RArm;
        }

        public void SetHandDominancePosition(bool isLeft, bool isRight)
        {
            if (IsLDom() == isLeft) return;
            
            HandMoveIn(IsLDom());
            leftHand.SetDom(isLeft);
            rightHand.SetDom(isRight);

            LArm.localRotation = Quaternion.Euler(isLeft ? 0f : 10f, 0f, 0f);
            RArm.localRotation = Quaternion.Euler(isRight ? 0f : 10f, 0f, 0f);
        }

        public void HandMoveOutAndIn(bool isLHandDom)
        {
            if (GetDominantHand().IsInAnimation() || GetDominantHand().IsOutStretched()) return;
            GetDominantHand().SetInAnimation(true);
            StartCoroutine(AnimationHandOutAndIn(isLHandDom ? LArm : RArm));
        }

        public void ToggleHandMoveOutOrIn(bool? isLHandDom)
        {
            if (isLHandDom == null) isLHandDom = IsLDom();
            if (GetDominantHand().IsOutStretched())
                HandMoveIn((bool)isLHandDom);
            else
                HandMoveOut((bool)isLHandDom);
        }

        private void HandMoveOut(bool isLHandDom)
        {
            if (GetDominantHand().IsInAnimation() || GetDominantHand().IsOutStretched()) return;
            GetDominantHand().SetInAnimation(true);
            StartCoroutine(AnimationMoveHandOut(isLHandDom ? LArm : RArm));
        }
        private void HandMoveIn(bool isLHandDom)
        {
            if (GetDominantHand().IsInAnimation() || !GetDominantHand().IsOutStretched()) return;
            GetDominantHand().SetInAnimation(true);
            StartCoroutine(AnimationMoveHandIn(isLHandDom ? LArm : RArm));
        }

        private IEnumerator AnimationMoveHandOut(Transform handTransform)
        {
            handOutStretched = true;
            recentHandStartPos = handTransform.localPosition;
            float time = 0f;
            float elapsedTime = 0f;

            while (elapsedTime < duration/2)
            {
                time += Time.deltaTime * frequency;
                float offset = Mathf.Sin(time) * amplitude;
                handTransform.localPosition = recentHandStartPos + new Vector3(0f, 0f, offset);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            inOutStretchHandAnimation = false;
            handOutStretched = true;
        }

        private IEnumerator AnimationMoveHandIn(Transform handTransform)
        {
            handOutStretched = true;
            float time = 0f;
            float elapsedTime = 0f;

            while (elapsedTime < duration / 2)
            {
                time += Time.deltaTime * frequency;
                float offset = -(1 - Mathf.Cos(time)) * amplitude;
                handTransform.localPosition = recentHandStartPos + new Vector3(0f, 0f, offset);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            handTransform.localPosition = recentHandStartPos;
            inOutStretchHandAnimation = false;
            handOutStretched = false;
        }

        private IEnumerator AnimationHandOutAndIn(Transform handTransform)
        {
            handOutStretched = true;
            Vector3 recentHandStartPos = handTransform.localPosition;
            float time = 0f;
            float elapsedTime = 0f;

            while(elapsedTime < duration)
            {
                time += Time.deltaTime * frequency;
                float offset = Mathf.Sin(time) * amplitude;
                handTransform.localPosition = recentHandStartPos + new Vector3(0f, 0f, offset);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            handTransform.localPosition = recentHandStartPos;
            inOutStretchHandAnimation = false;
            handOutStretched = true;
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
