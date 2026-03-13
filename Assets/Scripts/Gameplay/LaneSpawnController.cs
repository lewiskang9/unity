using SpectrumDrift.Core;
using SpectrumDrift.Data;
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

        [Header("References")]
        [SerializeField] private Transform runnerTarget;
        [SerializeField] private StageLayoutConfig stageLayout;

        [Header("Prefabs")]
        [SerializeField] private SpawnPrefab[] absorbPrefabs;
        [SerializeField] private GameObject hazardPrefab;
        [SerializeField] private GameObject[] gatePrefabs;

        [Header("Spawn Settings")]
        [SerializeField, Min(3)] private int lanes = 3;
        [SerializeField, Min(5f)] private float segmentLength = 10f;
        [SerializeField, Min(1f)] private float laneGap = 3f;
        [SerializeField, Min(3)] private int warmupSegments = 6;
        [SerializeField, Min(10f)] private float keepAheadDistance = 60f;

        private float _nextSpawnZ;
        private int _spawnedSegments;

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
            StageLayoutConfig.Segment segment = stageLayout != null
                ? stageLayout.GetSegmentByIndex(_spawnedSegments)
                : default;

            for (int lane = 0; lane < lanes; lane++)
            {
                float laneOffset = (lane - ((lanes - 1) * 0.5f)) * laneGap;
                Vector3 position = new Vector3(laneOffset, 0.5f, _nextSpawnZ);

                GameObject prefab = SelectPrefab(segment);
                if (prefab == null)
                {
                    continue;
                }

                Instantiate(prefab, position, Quaternion.identity, transform);
            }

            TrySpawnGate(segment);
            _nextSpawnZ += segmentLength;
            _spawnedSegments++;
        }

        private void TrySpawnGate(StageLayoutConfig.Segment segment)
        {
            if (gatePrefabs == null || gatePrefabs.Length == 0)
            {
                return;
            }

            float chance = stageLayout != null ? segment.gateSpawnChance : 0.08f;
            if (Random.value > chance)
            {
                return;
            }

            int gateIndex = Random.Range(0, gatePrefabs.Length);
            GameObject gatePrefab = gatePrefabs[gateIndex];
            if (gatePrefab == null)
            {
                return;
            }

            Vector3 gatePosition = new Vector3(0f, 1f, _nextSpawnZ + (segmentLength * 0.5f));
            Instantiate(gatePrefab, gatePosition, Quaternion.identity, transform);
        }

        private GameObject SelectPrefab(StageLayoutConfig.Segment segment)
        {
            float hazardChance = stageLayout != null ? segment.hazardChance : 0.2f;
            if (hazardPrefab != null && Random.value < hazardChance)
            {
                return hazardPrefab;
            }

            if (absorbPrefabs == null || absorbPrefabs.Length == 0)
            {
                return null;
            }

            if (stageLayout != null && Random.value < segment.preferredStateChance)
            {
                for (int i = 0; i < absorbPrefabs.Length; i++)
                {
                    if (absorbPrefabs[i].state == segment.preferredState && absorbPrefabs[i].prefab != null)
                    {
                        return absorbPrefabs[i].prefab;
                    }
                }
            }

            int index = Random.Range(0, absorbPrefabs.Length);
            return absorbPrefabs[index].prefab;
        }
    }
}
