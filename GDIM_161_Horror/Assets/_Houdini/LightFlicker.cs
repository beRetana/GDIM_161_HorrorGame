using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 0.1f;     
    public bool useRandomFlicker = true;  

    private Light flickerLight;
    private float baseIntensity;
    private float timer;

    void Start()
    {
        flickerLight = GetComponent<Light>();
        baseIntensity = flickerLight.intensity;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= flickerSpeed)
        {
            timer = 0f;

            if (useRandomFlicker)
            {
                flickerLight.intensity = Random.Range(minIntensity, maxIntensity);
            }
            else
            {
                float sinValue = Mathf.Sin(Time.time * 20f) * 0.5f + 0.5f;
                flickerLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, sinValue);
            }
        }
    }
}
