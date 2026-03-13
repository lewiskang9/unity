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
        public UnityEvent onPlayerCollapsed;
        public UnityEvent<float> onSplitTimeChanged;
        public UnityEvent<float> onMagnetTimeChanged;

        private readonly Collider[] _magnetHits = new Collider[32];

        private float _mass;
        private int _score;
        private int _comboCount;
        private float _comboExpireAt;
        private bool _awaitingPerfectTransitionAbsorb;
        private bool _isCollapsed;
        private float _splitRemaining;
        private float _magnetRemaining;

        public ElementState CurrentState => currentState;
        public float CurrentMass => _mass;
        public int CurrentScore => _score;
        public int CurrentCombo => _comboCount;
        public bool IsCollapsed => _isCollapsed;
        public bool IsSplitActive => _splitRemaining > 0f;
        public bool IsMagnetActive => _magnetRemaining > 0f;

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
            if (balance == null || _isCollapsed)
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

            TickGateAbilities();
        }

        public void Absorb(ElementState resourceState, int value = 1)
        {
            if (balance == null || _isCollapsed)
            {
                return;
            }

            int effectiveValue = IsSplitActive ? Mathf.RoundToInt(value * balance.splitAbsorbMultiplier) : value;
            bool matched = resourceState == currentState;

            if (matched)
            {
                ApplyMassDelta(balance.massGainOnMatch * effectiveValue);
                UpdateCombo();

                float comboSteps = Mathf.Floor(_comboCount / balance.comboStepThreshold);
                float scoreMultiplier = 1f + (comboSteps * balance.comboMultiplierPerStep);

                if (currentState == ElementState.YellowOvercharge)
                {
                    scoreMultiplier += balance.overchargeScoreMultiplier;
                }

                int gained = Mathf.RoundToInt(balance.baseAbsorbScore * effectiveValue * scoreMultiplier);
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

        public void ApplyNeutralHit(float massPenalty, int scorePenalty)
        {
            if (balance == null || _isCollapsed)
            {
                return;
            }

            ApplyMassDelta(-Mathf.Abs(massPenalty));
            _score = Mathf.Max(0, _score - Mathf.Abs(scorePenalty));
            _comboCount = 0;
            onComboChanged?.Invoke(_comboCount);
            onScoreChanged?.Invoke(_score);
        }

        public void ChangeState(ElementState nextState)
        {
            if (_isCollapsed)
            {
                return;
            }

            currentState = nextState;
            _awaitingPerfectTransitionAbsorb = true;

            if (balance != null && currentState == ElementState.GreenRecover)
            {
                ApplyMassDelta(balance.recoverBonusMass);
            }

            RefreshRunnerSpeed();
            onStateChanged?.Invoke(currentState);
        }

        public void ActivateSplit()
        {
            if (balance == null)
            {
                return;
            }

            _splitRemaining = Mathf.Max(_splitRemaining, balance.splitDuration);
            onSplitTimeChanged?.Invoke(_splitRemaining);
        }

        public void ActivateMagnet()
        {
            if (balance == null)
            {
                return;
            }

            _magnetRemaining = Mathf.Max(_magnetRemaining, balance.magnetDuration);
            onMagnetTimeChanged?.Invoke(_magnetRemaining);
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

        private void TickGateAbilities()
        {
            if (_splitRemaining > 0f)
            {
                _splitRemaining -= Time.deltaTime;
                onSplitTimeChanged?.Invoke(Mathf.Max(0f, _splitRemaining));
            }

            if (_magnetRemaining > 0f)
            {
                _magnetRemaining -= Time.deltaTime;
                onMagnetTimeChanged?.Invoke(Mathf.Max(0f, _magnetRemaining));
                PullMagnetAbsorbables();
            }
        }

        private void PullMagnetAbsorbables()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, balance.magnetRadius, _magnetHits);
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _magnetHits[i];
                if (hit == null)
                {
                    continue;
                }

                AbsorbableObject absorbable = hit.GetComponent<AbsorbableObject>();
                if (absorbable == null || absorbable.ResourceState != currentState)
                {
                    continue;
                }

                absorbable.TryAbsorbBy(this);
            }
        }

        private void ApplyMassDelta(float delta)
        {
            if (balance == null)
            {
                _mass = Mathf.Max(0.01f, _mass + delta);
            }
            else
            {
                _mass = Mathf.Clamp(_mass + delta, 0.01f, balance.maxMass);
            }

            transform.localScale = Vector3.one * _mass;
            onMassChanged?.Invoke(_mass);

            float collapseThreshold = balance != null ? balance.minMass : 0.2f;
            if (!_isCollapsed && _mass <= collapseThreshold)
            {
                Collapse();
            }
        }

        private void Collapse()
        {
            _isCollapsed = true;
            if (runner != null)
            {
                runner.StopRun();
            }

            onPlayerCollapsed?.Invoke();
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
            onSplitTimeChanged?.Invoke(_splitRemaining);
            onMagnetTimeChanged?.Invoke(_magnetRemaining);
        }
    }
}
