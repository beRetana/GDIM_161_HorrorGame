using UnityEngine;
using System.Collections;

namespace Player
{
    public class Arms : MonoBehaviour
    {
        [SerializeField] Transform LArm;
        [SerializeField] Transform RArm;

        private void Start()
        {
            SetHandDominancePosition(false, true);
        }

        public void SetHandDominancePosition(bool isLeft, bool isRight)
        {
            LArm.localRotation = Quaternion.Euler(isLeft ? 0f : 10f, 0f, 0f);
            RArm.localRotation = Quaternion.Euler(isRight ? 0f : 10f, 0f, 0f);
        }

        /*public void OutStretchHand()
        {

        }

        private IEnumerator()
        {

        }*/

    }
}
