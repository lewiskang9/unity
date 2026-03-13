using SpectrumDrift.Core;
using SpectrumDrift.Data;
using UnityEngine;
using UnityEngine.Events;

namespace SpectrumDrift.Gameplay
{
    public class PlayerStateController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RunnerController runner;
        [SerializeField] private StageBalanceConfig balance;

        [Header("Runtime")]
        [SerializeField] private ElementState currentState = ElementState.BlueCalm;

        public UnityEvent<ElementState> onStateChanged;
        public UnityEvent<float> onMassChanged;
        public UnityEvent<int> onComboChanged;
        public UnityEvent<int> onScoreChanged;

        private float _mass;
        private int _score;
        private int _comboCount;
        private float _comboExpireAt;
        private bool _awaitingPerfectTransitionAbsorb;

        public ElementState CurrentState => currentState;
        public float CurrentMass => _mass;
        public int CurrentScore => _score;

        private void Awake()
        {
            if (runner == null)
            {
                runner = GetComponent<RunnerController>();
            }

            _mass = balance != null ? balance.startMass : 1f;
            NotifyAll();
            RefreshRunnerSpeed();
        }

        private void Update()
        {
            if (balance == null)
            {
                return;
            }

            if (_comboCount > 0 && Time.time > _comboExpireAt)
            {
                _comboCount = 0;
                onComboChanged?.Invoke(_comboCount);
            }

            if (currentState == ElementState.RedHeat)
            {
                ApplyMassDelta(-balance.heatMassDrainPerSecond * Time.deltaTime);
            }
        }

        public void Absorb(ElementState resourceState, int value = 1)
        {
            if (balance == null)
            {
                return;
            }

            bool matched = resourceState == currentState;

            if (matched)
            {
                ApplyMassDelta(balance.massGainOnMatch * value);
                UpdateCombo();

                float scoreMultiplier = 1f + (Mathf.Floor(_comboCount / balance.comboStepThreshold) * balance.comboMultiplierPerStep);
                if (currentState == ElementState.YellowOvercharge)
                {
                    scoreMultiplier += balance.overchargeScoreMultiplier;
                }

                int gained = Mathf.RoundToInt(balance.baseAbsorbScore * value * scoreMultiplier);
                _score += gained;

                if (_awaitingPerfectTransitionAbsorb)
                {
                    _score += Mathf.RoundToInt(balance.perfectTransitionBonus);
                    _awaitingPerfectTransitionAbsorb = false;
                }
            }
            else
            {
                ApplyMassDelta(-(balance.massLossOnMismatch * value));
                _score = Mathf.Max(0, _score - Mathf.RoundToInt(balance.mismatchPenaltyScore * value));
                _comboCount = 0;
                onComboChanged?.Invoke(_comboCount);
                _awaitingPerfectTransitionAbsorb = false;
            }

            onScoreChanged?.Invoke(_score);
        }

        public void ChangeState(ElementState nextState)
        {
            currentState = nextState;
            _awaitingPerfectTransitionAbsorb = true;

            if (balance != null && currentState == ElementState.GreenRecover)
            {
                ApplyMassDelta(balance.recoverBonusMass);
            }

            RefreshRunnerSpeed();
            onStateChanged?.Invoke(currentState);
        }

        public int CalculateFinishScore()
        {
            if (balance == null)
            {
                return _score;
            }

            int massBonus = Mathf.RoundToInt(_mass * balance.remainingMassScoreMultiplier);
            return _score + massBonus;
        }

        private void ApplyMassDelta(float delta)
        {
            _mass = Mathf.Clamp(_mass + delta, balance.minMass, balance.maxMass);
            transform.localScale = Vector3.one * _mass;
            onMassChanged?.Invoke(_mass);
        }

        private void UpdateCombo()
        {
            _comboCount++;
            _comboExpireAt = Time.time + balance.comboWindowSeconds;
            onComboChanged?.Invoke(_comboCount);
        }

        private void RefreshRunnerSpeed()
        {
            if (runner == null || balance == null)
            {
                return;
            }

            float speedBonus = currentState == ElementState.RedHeat ? balance.heatSpeedBonus : 0f;
            runner.SetForwardSpeedModifier(speedBonus);
        }

        private void NotifyAll()
        {
            onStateChanged?.Invoke(currentState);
            onMassChanged?.Invoke(_mass);
            onComboChanged?.Invoke(_comboCount);
            onScoreChanged?.Invoke(_score);
        }
    }
}
