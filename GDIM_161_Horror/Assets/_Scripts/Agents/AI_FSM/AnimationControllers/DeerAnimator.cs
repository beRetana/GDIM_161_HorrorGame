using UnityEngine;

namespace AI
{
    public class DeerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private const string JUMP = "JUMP", SPEED = "SPEED";

        private void Start() {if (_animator == null) TryGetComponent<Animator>(out _animator); }

        public void OnJump() { _animator.SetTrigger(JUMP); }

        public void SetSpeed(float speed) { _animator.SetFloat(SPEED, speed); }
    }
}
