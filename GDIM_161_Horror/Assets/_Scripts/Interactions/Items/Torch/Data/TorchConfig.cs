using Steamworks;
using UnityEngine;

[CreateAssetMenu(menuName = "Torch/Torch Configuration")]
public class TorchConfig : ScriptableObject
{
    [Header("Burn Settings")]
    public float woodLifeMinutes = 10f;
    public float maxBurnVariance = 0.2f;
    public int pyrolysisIncrements = 20;
    public float tickSpeedMultiplier = 1.05f;

    [Header("Fire Settings")]
    public float burnOutTime = 2f;
    public float flameDecayRate = -1.5f;
    public float lightDecayRate = -.5f;
    public float growRate = 1.5f;
    public float growCurveB = 5f;

    [Header("Visual Settings")]
    public float maxFlameSize = 1f;
    public float maxLightIntensity = 1f;
    public float smotherRadius = 0.5f;
}
