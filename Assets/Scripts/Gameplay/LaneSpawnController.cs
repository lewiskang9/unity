using SpectrumDrift.Core;
using UnityEngine;

namespace SpectrumDrift.Gameplay
{
    public class LaneSpawnController : MonoBehaviour
    {
        [System.Serializable]
        public struct SpawnPrefab
        {
            public ElementState state;
            public GameObject prefab;
        }

        [SerializeField] private Transform runnerTarget;
        [SerializeField] private SpawnPrefab[] absorbPrefabs;
        [SerializeField] private GameObject hazardPrefab;
        [SerializeField, Min(3)] private int lanes = 3;
        [SerializeField, Min(5f)] private float segmentLength = 10f;
        [SerializeField, Min(1f)] private float laneGap = 3f;
        [SerializeField, Min(3)] private int warmupSegments = 6;
        [SerializeField, Min(10f)] private float keepAheadDistance = 60f;
        [SerializeField, Range(0f, 1f)] private float hazardChance = 0.2f;

        private float _nextSpawnZ;

        private void Start()
        {
            if (runnerTarget == null)
            {
                PlayerStateController player = FindObjectOfType<PlayerStateController>();
                runnerTarget = player != null ? player.transform : null;
            }

            for (int i = 0; i < warmupSegments; i++)
            {
                SpawnSegment();
            }
        }

        private void Update()
        {
            if (runnerTarget == null)
            {
                return;
            }

            while (_nextSpawnZ < runnerTarget.position.z + keepAheadDistance)
            {
                SpawnSegment();
            }
        }

        private void SpawnSegment()
        {
            for (int lane = 0; lane < lanes; lane++)
            {
                float laneOffset = (lane - ((lanes - 1) * 0.5f)) * laneGap;
                Vector3 position = new Vector3(laneOffset, 0.5f, _nextSpawnZ);

                GameObject prefab = SelectPrefab();
                if (prefab == null)
                {
                    continue;
                }

                Instantiate(prefab, position, Quaternion.identity, transform);
            }

            _nextSpawnZ += segmentLength;
        }

        private GameObject SelectPrefab()
        {
            bool spawnHazard = hazardPrefab != null && Random.value < hazardChance;
            if (spawnHazard)
            {
                return hazardPrefab;
            }

            if (absorbPrefabs == null || absorbPrefabs.Length == 0)
            {
                return null;
            }

            int index = Random.Range(0, absorbPrefabs.Length);
            return absorbPrefabs[index].prefab;
        }
    }
}
