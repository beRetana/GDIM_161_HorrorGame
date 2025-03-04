using UnityEngine;

public class FogTriggerZone : MonoBehaviour
{
    public float newFogDensity = 0.1f;
    public float transitionSpeed = 1f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FogController fogController = FindObjectOfType<FogController>(); 
            if (fogController != null)
            {
                fogController.SetFogDensity(newFogDensity, transitionSpeed);
            }
            else
            {
                Debug.LogWarning("FogController not found in the scene!");
            }
        }
    }
}
