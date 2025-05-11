using UnityEditor;
using UnityEngine;

public class TorchData : MonoBehaviour
{
    public float BurnTimer { get; private set; }
    public float BurnRatio => BurnTimer / (config.woodLifeMinutes * 60f);

    private TorchConfig config;
    private float pyrolysisTimer;
    private float burnVelocity = 1f;

    public void Initialize(TorchConfig config)
    {
        this.config = config;
        BurnTimer = config.woodLifeMinutes * 60f;
        pyrolysisTimer = BurnTimer / config.pyrolysisIncrements;
    }

    public void UpdateTimers(float deltaTime)
    {
        burnVelocity = CalculateSmoothRandom(burnVelocity);
        pyrolysisTimer -= burnVelocity * deltaTime;
        BurnTimer -= burnVelocity * deltaTime * config.tickSpeedMultiplier;
    }

    public bool ShouldUpdatePyrolysis() => pyrolysisTimer <= 0f;
    public void ResetPyrolysisTimer() => pyrolysisTimer = BurnTimer / config.pyrolysisIncrements;

    public float CalculateSmoothRandom(float x)
    {
        float acceleration = Random.Range(-.05f, .05f);
        return Mathf.Clamp(x + acceleration, 
                            1 - config.maxBurnVariance, 
                            1 + config.maxBurnVariance);
    }
}
