using UnityEngine;

public class LIGHTBOUNCE : MonoBehaviour
{
    public Light pointLight;
    public float bounceSpeed = 2f;
    public float minIntensity = 1f;
    public float maxIntensity = 5;

    private void Update()
    {
        if (pointLight != null)
        {
            pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * bounceSpeed, 1));
        }

    }
}
