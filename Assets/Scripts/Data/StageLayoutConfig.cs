using SpectrumDrift.Core;
using UnityEngine;

namespace SpectrumDrift.Data
{
    [CreateAssetMenu(fileName = "StageLayoutConfig", menuName = "Spectrum Drift/Stage Layout Config")]
    public class StageLayoutConfig : ScriptableObject
    {
        [System.Serializable]
        public struct Segment
        {
            [Min(1)] public int segmentCount;
            public ElementState preferredState;
            [Range(0f, 1f)] public float preferredStateChance;
            [Range(0f, 1f)] public float hazardChance;
            [Range(0f, 1f)] public float gateSpawnChance;
        }

        public Segment[] segments;

        public Segment GetSegmentByIndex(int spawnedSegmentCount)
        {
            if (segments == null || segments.Length == 0)
            {
                return new Segment
                {
                    segmentCount = int.MaxValue,
                    preferredState = ElementState.BlueCalm,
                    preferredStateChance = 0.5f,
                    hazardChance = 0.2f,
                    gateSpawnChance = 0.1f
                };
            }

            int walked = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                walked += Mathf.Max(1, segments[i].segmentCount);
                if (spawnedSegmentCount < walked)
                {
                    return segments[i];
                }
            }

            return segments[segments.Length - 1];
        }
    }
}
