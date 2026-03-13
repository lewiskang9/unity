using UnityEngine;
using UnityEngine.Events;

namespace SpectrumDrift.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class FinishZone : MonoBehaviour
    {
        public UnityEvent onReachedFinish;

        private bool _triggered;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered)
            {
                return;
            }

            PlayerStateController player = other.GetComponent<PlayerStateController>();
            if (player == null)
            {
                return;
            }

            _triggered = true;
            onReachedFinish?.Invoke();
        }
    }
}
