using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TorchFireController : MonoBehaviour
{
    [SerializeField] private ParticleSystem flameVFX;
    [SerializeField] private Transform[] flameParts;
    [SerializeField] private Light torchLight;

    private TorchConfig config;
    private bool isLit = false;

    public bool IsLit => isLit;
    public void Initialize(TorchConfig torchConfig)
    {
        config = torchConfig;
        if (flameVFX == null || torchLight == null)
        {
            Debug.LogError("TorchFireController: Missing VFX or Light references.");
            return;
        }
        torchLight.intensity = 0f;
        SetFlameSize(0);
        flameVFX.Stop();
    }

    public void Light()
    {
        if (isLit) return;
        StartCoroutine(IgniteCoroutine());
    }

    public void Extinguish()
    {
        if(!isLit) return;
        StartCoroutine(BurnOutCoroutine());
    }

    public void Smother()
    {
        StopAllCoroutines();
        SetFlameSize(0);
        torchLight.intensity = 0f;
        flameVFX.Stop();
        isLit = false;
    }

    private IEnumerator IgniteCoroutine()
    {
        isLit = true;
        flameVFX.Play();

        float lightIntensityM = -config.growRate / Mathf.Log(0.1f, config.growCurveB);

        for (float delta = 0; delta < 5f; delta += Time.deltaTime)
        {
            float flameSize = 1.05f / (1 + config.growCurveB * Mathf.Exp(-config.growRate * delta));
            SetFlameSize(flameSize);
            torchLight.intensity = config.maxLightIntensity * (delta * lightIntensityM);
            if (flameSize >= 1f) break;
            yield return null;
        }

        SetFlameSize(1f);
        torchLight.intensity = config.maxLightIntensity;
    }
    
    private IEnumerator BurnOutCoroutine()
    {
        for(float delta = 0; delta < config.burnOutTime; delta += Time.deltaTime)
        {
            float flameSize = config.maxFlameSize * Mathf.Exp(config.flameDecayRate * delta);
            float lightIntensity = config.maxLightIntensity * Mathf.Exp(config.lightDecayRate * delta);

            SetFlameSize(flameSize);
            torchLight.intensity = lightIntensity;
            yield return null;
        }
    }

    private void SetFlameSize(float size)
    {
        foreach (var part in flameParts)
        {
            part.localScale = Vector3.one * size;
        }
    }
}
