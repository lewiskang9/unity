using UnityEngine;

namespace SpectrumDrift.Gameplay
{
    public class WebGameBootstrap : MonoBehaviour
    {
        [SerializeField, Min(15)] private int targetFrameRate = 60;
        [SerializeField] private bool runInBackground = true;

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            Application.runInBackground = runInBackground;
            QualitySettings.vSyncCount = 0;
        }
    }
}
