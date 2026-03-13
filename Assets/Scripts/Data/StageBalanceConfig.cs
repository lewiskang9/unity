using UnityEngine;

namespace SpectrumDrift.Data
{
    [CreateAssetMenu(fileName = "StageBalanceConfig", menuName = "Spectrum Drift/Stage Balance Config")]
    public class StageBalanceConfig : ScriptableObject
    {
        [Header("Mass")]
        [Min(0.1f)] public float startMass = 1f;
        [Min(0.01f)] public float massGainOnMatch = 0.12f;
        [Min(0.01f)] public float massLossOnMismatch = 0.18f;
        [Min(0.1f)] public float minMass = 0.4f;
        [Min(0.2f)] public float maxMass = 4.5f;

        [Header("Score")]
        [Min(1f)] public float baseAbsorbScore = 10f;
        [Min(0f)] public float mismatchPenaltyScore = 5f;

        [Header("Combo")]
        [Min(0.1f)] public float comboWindowSeconds = 2.25f;
        [Min(1f)] public float comboStepThreshold = 5f;
        [Min(0f)] public float comboMultiplierPerStep = 0.25f;

        [Header("State Specific")]
        [Min(0f)] public float heatSpeedBonus = 2.5f;
        [Min(0f)] public float heatMassDrainPerSecond = 0.08f;
        [Min(0f)] public float recoverBonusMass = 0.08f;
        [Min(0f)] public float overchargeScoreMultiplier = 0.35f;

        [Header("Finish")]
        [Min(0f)] public float remainingMassScoreMultiplier = 35f;
        [Min(0f)] public float perfectTransitionBonus = 50f;
    }
}
