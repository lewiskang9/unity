using UnityEngine;

namespace SpectrumDrift.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class HazardZone : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float massPenalty = 0.2f;
        [SerializeField, Min(0)] private int scorePenalty = 15;
        [SerializeField] private bool singleUse = false;

        private bool _used;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (singleUse && _used)
            {
                return;
            }

            PlayerStateController player = other.GetComponent<PlayerStateController>();
            if (player == null)
            {
                return;
            }

            player.ApplyNeutralHit(massPenalty, scorePenalty);
            _used = true;
        }
    }
}
