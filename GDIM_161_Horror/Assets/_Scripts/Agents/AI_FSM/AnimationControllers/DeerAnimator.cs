using UnityEngine;

namespace AI
{
    public class DeerAnimator : MonoBehaviour
    {
        private Animator _animator;

        private const string JUMP = "JUMP", SPEED = "SPEED";

        private void Start() { _animator = GetComponent<Animator>(); }

        public void OnJump() { _animator.SetTrigger(JUMP); }

        public void SetSpeed(float speed) { _animator.SetFloat(SPEED, speed); }
    }
}
