using UnityEngine;

namespace SpectrumDrift.Gameplay
{
    public class AutoDestroyBehindRunner : MonoBehaviour
    {
        [SerializeField] private Transform runnerTarget;
        [SerializeField, Min(5f)] private float destroyDistance = 20f;

        private void Start()
        {
            if (runnerTarget == null)
            {
                PlayerStateController player = FindObjectOfType<PlayerStateController>();
                runnerTarget = player != null ? player.transform : null;
            }
        }

        private void Update()
        {
            if (runnerTarget == null)
            {
                return;
            }

            if (runnerTarget.position.z - transform.position.z > destroyDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
