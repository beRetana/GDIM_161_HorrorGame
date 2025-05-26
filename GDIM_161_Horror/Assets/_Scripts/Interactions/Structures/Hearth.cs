using UnityEngine;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
namespace Interactions
{
    public class Hearth : MonoBehaviour, IFireable
    {
        [Header("Gameplay Stuff")]
        [SerializeField, Range(600f, 2400f)] private float maxBurnTime = 1200f; // In seconds

        [Header("Model Stuff")]
        [SerializeField] private ParticleSystem fireVFX; // Fire particle Effect
        [SerializeField] private Light fireLight; // Light Source
        [SerializeField] private Transform flameRed;
        [SerializeField] private Transform flameOrange;
        [SerializeField] private Transform flameYellow;

        public float BurnTimer { get; private set; }
        public bool Lit { get; private set; }

        private float maxLightIntensity;
        private float lightIntensity = -1f;

        public void Start()
        {
            maxLightIntensity = fireLight.intensity;

            StartFire();
        }
        public bool IsLit() { return Lit; }
        public void LightFlame() { } //pass

        private void StartFire()
        {
            Lit = true;
            BurnTimer = maxBurnTime;
            fireVFX.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (BurnTimer < 0)
            {
                if (Lit) BurnOut();
                return;
            }

            BurnTimer -= Time.deltaTime;

            UpdateLighting();
            UpdateSize();
        }

        /// <summary>
        /// (2x+1)(x-1)(x-1)
        /// </summary>
        private void UpdateSize()
        {
            float percent = (maxBurnTime - BurnTimer) / maxBurnTime;
            float scalar = (2 * percent + 1) * Mathf.Pow((percent - 1), 2);
            Vector3 newFlameScale = Vector3.one * scalar;

            flameRed.localScale = newFlameScale;
            flameOrange.localScale = newFlameScale;
            flameYellow.localScale = newFlameScale;
        }

        private void UpdateLighting()
        {
            SetVisualLightIntensity(maxBurnTime / BurnTimer);
        }

        private void SetVisualLightIntensity(float intensePercent)
        {
            lightIntensity = Mathf.Min(maxLightIntensity * intensePercent * intensePercent, maxLightIntensity);
            fireLight.intensity = lightIntensity;
        }

        private void BurnOut()
        {
            Debug.Log("Burnt Out...");
            Lit = false;
            fireVFX.gameObject.SetActive(false);
            fireLight.enabled = false;
        }
    }
}
