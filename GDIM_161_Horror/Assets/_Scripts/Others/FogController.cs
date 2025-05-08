using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color fogColor = Color.gray;
    public FogMode fogMode = FogMode.ExponentialSquared; // Linear, Exponential, or ExponentialSquared
    public float fogDensity = 0.2f;
    public float fogStartDistance = 1f;
    public float fogEndDistance = 5f;
    public float changeSpeed = 0.5f;

    private bool isChanging = false; // Only update when needed
    void Start()
    {
        ApplyStartFogSettings();
    }

    void Update()
    {
        if (isChanging)
        {
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensity, changeSpeed * Time.deltaTime);

            // Stop updating if close enough to target value
            if (Mathf.Abs(RenderSettings.fogDensity - fogDensity) < 0.001f)
            {
                RenderSettings.fogDensity = fogDensity; // Snap to target
                isChanging = false; // Stop updates
            }
        }
    }

    public void ApplyStartFogSettings()
    {
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;

        if (fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.fogEndDistance = fogEndDistance;
        }
        else
        {
            RenderSettings.fogDensity = fogDensity;
        }
    }

    public void SetFogDensity(float newDensity, float newSpeed)
    {
        if (Mathf.Abs(RenderSettings.fogDensity - newDensity) > 0.001f)
        {
            fogDensity = newDensity;  // Set the new target density
            changeSpeed = newSpeed;   // Update the speed dynamically
            isChanging = true;        
        }
    }
}
