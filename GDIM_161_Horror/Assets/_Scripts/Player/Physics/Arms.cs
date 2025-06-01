using UnityEngine;
using System.Collections;
using System;
using StarterAssets;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;


namespace Player
{
    public class Arms : MonoBehaviour
    {
        public int assignedPlayerID { get; private set; }

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
            private HandStateEnum handState;
            public int assignedPlayerID { get; private set; }

            public Vector3 StartPos { get; private set; }
            public Vector3 AnimStartPos { get; private set; }
            
            public Hand(int playerID, Vector3 startPos)
            {
                assignedPlayerID = playerID;
                StartPos = startPos;
                AnimStartPos = startPos;

                handState = 000;
            }

            public void SetAnimStartPosition(Vector3 position)
            {
                AnimStartPos = position;
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
            assignedPlayerID = GetComponentInParent<FirstPersonController>().ID();
            SetUpHands();
        }

        private void SetUpHands()
        {
            leftHand = new Hand(assignedPlayerID, LArm.localPosition);
            rightHand = new Hand(assignedPlayerID, RArm.localPosition);
            SetInitHandPositions();
        }

        #region hand_helpers
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

        private Transform GetHandTransform(Hand thisHand)
        {
            if (thisHand == leftHand) return LArm;
            if (thisHand == rightHand) return RArm;
            return null;
        }

        public bool IsDomOutStretched()
        {
            return GetDominantHand().IsOutStretched();
        }

        #endregion hand_helpers

        #region hand_modifiers
        private void SetInitHandPositions()
        {
            leftHand.SetDom(false);
            rightHand.SetDom(true);
            LArm.localRotation = Quaternion.Euler(10f, 0f, 0f); 
            RArm.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        public void SetHandDominancePosition(bool isLeft, bool isRight)
        {
            if (IsLDom() == isLeft) return;
            
            HandMoveIn();
            leftHand.SetDom(isLeft);
            rightHand.SetDom(isRight);

            LArm.localRotation = Quaternion.Euler(isLeft ? 0f : 10f, 0f, 0f);
            RArm.localRotation = Quaternion.Euler(isRight ? 0f : 10f, 0f, 0f);
        }

        #endregion hand_modifiers

        #region animation_calls

        public void HandMoveOutAndIn(bool isOnLHand)
        {
            Hand thisHand = isOnLHand ? leftHand : rightHand;
            if (thisHand.IsInAnimation() || thisHand.IsOutStretched()) return;
            StartCoroutine(AnimationHandOutAndIn(thisHand));
        }

        public void ToggleHandMoveOutOrIn(bool? isLHandDom)
        {
            isLHandDom ??= IsLDom();
            if (GetDominantHand().IsOutStretched())
                HandMoveIn();
            else
                HandMoveOut();
        }
        private void HandMoveOut()
        {
            if (GetDominantHand().IsInAnimation() || GetDominantHand().IsOutStretched()) return;
            StartCoroutine(AnimationMoveHandOut());
        }
        private void HandMoveIn()
        {
            if (GetDominantHand().IsInAnimation() || !GetDominantHand().IsOutStretched()) return;
            StartCoroutine(AnimationMoveHandIn());
        }

        #endregion animation_calls

        #region animation_animation
        private IEnumerator AnimationHandOutAndIn(Hand thisHand) // used ALSO for picking up items with auxilary hand
        {
            Transform handTransform = GetHandTransform(thisHand);
            PrepareHandForAnimation(thisHand, handTransform.localPosition);

            for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
            {
                float offset = Mathf.Sin(elapsedTime * frequency) * amplitude;
                handTransform.localPosition = thisHand.AnimStartPos + new Vector3(0f, 0f, offset);
                yield return null;
            }

            GoToAnimStartPos(thisHand, handTransform);
        }

        private IEnumerator AnimationMoveHandOut()
        {
            Transform handTransform = GetDominantHandTransform();
            Hand thisHand = PrepareDominantHandForAnimation(handTransform.localPosition);

            for (float elapsedTime = 0f; elapsedTime < duration/2; elapsedTime += Time.deltaTime)
            {
                float offset = Mathf.Sin(elapsedTime * frequency) * amplitude;
                handTransform.localPosition = thisHand.AnimStartPos + new Vector3(0f, 0f, offset);
                yield return null;
            }

            thisHand.SetInAnimation(false);
            thisHand.SetOutStretched(true);
        }

        private IEnumerator AnimationMoveHandIn()
        {
            Transform handTransform = GetDominantHandTransform();
            Hand thisHand = PrepareDominantHandForAnimation(handTransform.localPosition);


            for (float elapsedTime = 0f; elapsedTime < duration / 2; elapsedTime += Time.deltaTime)
            {
                float offset = -(1 - Mathf.Cos(elapsedTime * frequency)) * amplitude;
                handTransform.localPosition = thisHand.AnimStartPos + new Vector3(0f, 0f, offset);
                yield return null;
            }

            GoToStartPos(thisHand, handTransform);
        }

        #endregion animation_animation

        #region animation_helpers

        Hand PrepareDominantHandForAnimation(Vector3 recentLocalPos)
        {
            Hand thisHand = GetDominantHand();
            thisHand.SetOutStretched(true);
            thisHand.SetInAnimation(true);
            thisHand.SetAnimStartPosition(recentLocalPos);

            return thisHand;
        }

        void PrepareHandForAnimation(Hand thisHand, Vector3 handLocalPos)
        {
            thisHand.SetOutStretched(true);
            thisHand.SetInAnimation(true);
            thisHand.SetAnimStartPosition(handLocalPos);
        }

        void GoToAnimStartPos(Hand hand, Transform handTransform)
        {
            handTransform.localPosition = hand.AnimStartPos;
            hand.SetInAnimation(false);
            hand.SetOutStretched(false);
        }
        void GoToStartPos(Hand hand, Transform handTransform)
        {
            handTransform.localPosition = hand.StartPos;
            hand.SetInAnimation(false);
            hand.SetOutStretched(false);
        }

        #endregion animation_helpers

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
