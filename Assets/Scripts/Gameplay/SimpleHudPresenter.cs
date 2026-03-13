using SpectrumDrift.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SpectrumDrift.Gameplay
{
    public class SimpleHudPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerStateController player;
        [SerializeField] private StageGameManager stageGameManager;

        [Header("UI")]
        [SerializeField] private Text stateText;
        [SerializeField] private Text massText;
        [SerializeField] private Text comboText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text abilityText;
        [SerializeField] private Text resultText;

        private float _splitTime;
        private float _magnetTime;

        private void Awake()
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerStateController>();
            }

            if (stageGameManager == null)
            {
                stageGameManager = FindObjectOfType<StageGameManager>();
            }
        }

        private void OnEnable()
        {
            if (player != null)
            {
                player.onStateChanged.AddListener(HandleStateChanged);
                player.onMassChanged.AddListener(HandleMassChanged);
                player.onComboChanged.AddListener(HandleComboChanged);
                player.onScoreChanged.AddListener(HandleScoreChanged);
                player.onSplitTimeChanged.AddListener(HandleSplitTimeChanged);
                player.onMagnetTimeChanged.AddListener(HandleMagnetTimeChanged);
                player.onPlayerCollapsed.AddListener(HandleFail);
            }

            if (stageGameManager != null)
            {
                stageGameManager.onTimeChanged.AddListener(HandleTimeChanged);
                stageGameManager.onFinishScore.AddListener(HandleFinishScore);
            }
        }

        private void OnDisable()
        {
            if (player != null)
            {
                player.onStateChanged.RemoveListener(HandleStateChanged);
                player.onMassChanged.RemoveListener(HandleMassChanged);
                player.onComboChanged.RemoveListener(HandleComboChanged);
                player.onScoreChanged.RemoveListener(HandleScoreChanged);
                player.onSplitTimeChanged.RemoveListener(HandleSplitTimeChanged);
                player.onMagnetTimeChanged.RemoveListener(HandleMagnetTimeChanged);
                player.onPlayerCollapsed.RemoveListener(HandleFail);
            }

            if (stageGameManager != null)
            {
                stageGameManager.onTimeChanged.RemoveListener(HandleTimeChanged);
                stageGameManager.onFinishScore.RemoveListener(HandleFinishScore);
            }
        }

        private void HandleStateChanged(ElementState state)
        {
            if (stateText != null)
            {
                stateText.text = $"State: {state}";
            }
        }

        private void HandleMassChanged(float mass)
        {
            if (massText != null)
            {
                massText.text = $"Mass: {mass:0.00}";
            }
        }

        private void HandleComboChanged(int combo)
        {
            if (comboText != null)
            {
                comboText.text = $"Combo: {combo}";
            }
        }

        private void HandleScoreChanged(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        private void HandleTimeChanged(float remain)
        {
            if (timeText != null)
            {
                timeText.text = $"Time: {remain:0.0}";
            }
        }

        private void HandleSplitTimeChanged(float remain)
        {
            _splitTime = remain;
            RefreshAbilityText();
        }

        private void HandleMagnetTimeChanged(float remain)
        {
            _magnetTime = remain;
            RefreshAbilityText();
        }

        private void HandleFinishScore(int score)
        {
            if (resultText != null)
            {
                resultText.text = $"CLEAR! Final Score: {score}";
            }
        }

        private void HandleFail()
        {
            if (resultText != null)
            {
                resultText.text = "FAILED - COLLAPSED";
            }
        }

        private void RefreshAbilityText()
        {
            if (abilityText == null)
            {
                return;
            }

            abilityText.text = $"Split: {_splitTime:0.0}s | Magnet: {_magnetTime:0.0}s";
        }
    }
}
