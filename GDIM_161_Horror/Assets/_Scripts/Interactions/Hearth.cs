using UnityEngine;
using System.Collections;
namespace Interactions
{
    public class Hearth : MonoBehaviour
    {
        public float totalBurnTime = 300f; // In seconds
        private float remainingBurnTime;

        public float woodRemovalPenalty = 30f; // Taking wood removes time
        public bool isBurning = true;   

        public GameObject fireEffect; // Fire particle Effect
        public Light fireLight; // Light Source

        void Start()
        {
        remainingBurnTime = totalBurnTime;
        StartCoroutine(BurnFire());
        }

         IEnumerator BurnFire()
    {
        while (remainingBurnTime > 0)
        {
            yield return new WaitForSeconds(1f); // Reduces Time
            remainingBurnTime--;

            // Decrease fire intensity over time
            if (fireLight != null)
                fireLight.intensity = Mathf.Lerp(0, 2, remainingBurnTime / totalBurnTime);
        }

        ExtinguishFire();
    }

    public void TakeWood()
    {
        if (isBurning && remainingBurnTime > woodRemovalPenalty)
        {
            remainingBurnTime -= woodRemovalPenalty;
            Debug.Log("Wood taken! Fire will now burn for " + remainingBurnTime + " more seconds.");
        }
        else
        {
            Debug.Log("The fire is too weak to take more wood!");
        }
    }

    private void ExtinguishFire()
    {
        isBurning = false;
        Debug.Log("The fire has gone out!");
        
        if (fireEffect != null)
            fireEffect.SetActive(false); // Disable fire particles
        
        if (fireLight != null)
            fireLight.enabled = false; // Turn off fire light
    }
    }
}
