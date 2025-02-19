using UnityEngine;

namespace Interactions
{
    public class DotDisplay : MonoBehaviour
    {
        private const string _TRIGGER_NAME = "PlayAnim";
        private Animator _animator;

        public override string ToString()
        {
            return $"Dot: {gameObject.name}";
        }

        void Start()
        {
            _animator = GetComponent<Animator>(); 
            gameObject.SetActive(false);
        }

        public void ActivateDot()
        {
            Debug.Log(ToString());
            gameObject.SetActive(true);
            _animator.ResetTrigger(_TRIGGER_NAME);
            _animator.SetTrigger(_TRIGGER_NAME);
        }

        public void DeactivateDot()
        { 
            gameObject.SetActive(false);
        }
    }
}
