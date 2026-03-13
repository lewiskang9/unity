using UnityEngine;
using UnityEngine.Events;

namespace SpectrumDrift.Gameplay
{
    public class StageGameManager : MonoBehaviour
    {
        [SerializeField] private RunnerController runner;
        [SerializeField] private PlayerStateController player;
        [SerializeField, Min(10f)] private float stageDuration = 60f;

        public UnityEvent<float> onTimeChanged;
        public UnityEvent<int> onFinishScore;
        public UnityEvent onStageClear;
        public UnityEvent onStageFail;

        private float _remainingTime;
        private bool _ended;

        private void Awake()
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerStateController>();
            }

            if (runner == null)
            {
                runner = FindObjectOfType<RunnerController>();
            }

            _remainingTime = stageDuration;
        }

        private void OnEnable()
        {
            if (player != null)
            {
                player.onPlayerCollapsed.AddListener(HandlePlayerCollapsed);
            }
        }

        private void OnDisable()
        {
            if (player != null)
            {
                player.onPlayerCollapsed.RemoveListener(HandlePlayerCollapsed);
            }
        }

        private void Update()
        {
            if (_ended)
            {
                return;
            }

            _remainingTime -= Time.deltaTime;
            onTimeChanged?.Invoke(Mathf.Max(0f, _remainingTime));

            if (_remainingTime <= 0f)
            {
                FinishStage();
            }
        }

        public void FinishStage()
        {
            if (_ended)
            {
                return;
            }

            _ended = true;
            if (runner != null)
            {
                runner.StopRun();
            }

            int finishScore = player != null ? player.CalculateFinishScore() : 0;
            onFinishScore?.Invoke(finishScore);
            onStageClear?.Invoke();
        }

        private void HandlePlayerCollapsed()
        {
            if (_ended)
            {
                return;
            }

            _ended = true;
            if (runner != null)
            {
                runner.StopRun();
            }

            onStageFail?.Invoke();
        }
    }
}
