using SpectrumDrift.Core;
using UnityEngine;

namespace SpectrumDrift.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class AbsorbableObject : MonoBehaviour
    {
        [SerializeField] private ElementState resourceState;
        [SerializeField, Min(1)] private int absorbValue = 1;
        [SerializeField] private bool destroyOnAbsorb = true;

        private bool _consumed;

        public ElementState ResourceState => resourceState;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerStateController player = other.GetComponent<PlayerStateController>();
            if (player == null)
            {
                return;
            }

            TryAbsorbBy(player);
        }

        public bool TryAbsorbBy(PlayerStateController player)
        {
            if (_consumed || player == null || player.IsCollapsed)
            {
                return false;
            }

            _consumed = true;
            player.Absorb(resourceState, absorbValue);

            if (destroyOnAbsorb)
            {
                Destroy(gameObject);
            }

            return true;
        }
    }
}
