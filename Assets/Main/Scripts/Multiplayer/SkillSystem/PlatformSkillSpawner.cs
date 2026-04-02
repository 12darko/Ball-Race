using Fusion;
using UnityEngine;
using System.Collections.Generic;

namespace Multiplayer.SkillSystem
{
    public class PlatformSkillSpawner : NetworkBehaviour
    {
        [Header("Specific spawn'lar (opsiyonel)")]
        [SerializeField] private SpecificSpawnEntry[] specificSpawns;
        public SpecificSpawnEntry[] SpecificSpawns => specificSpawns;

        [System.Serializable]
        public class SpecificSpawnEntry
        {
            public SkillData skill;
            public SkillSpawnPoint point;
        }

        public override void Spawned()
        {
            if (!HasStateAuthority) return;

            var skillManager = FindFirstObjectByType<SkillSpawnManager>();
            if (skillManager == null)
            {
                Debug.LogWarning("[PlatformSkillSpawner] SkillSpawnManager bulunamadı!");
                return;
            }

            skillManager.SpawnForPlatform(this);
        }

        public List<SkillSpawnPoint> GetSpawnPoints()
        {
            var result = new List<SkillSpawnPoint>();

            foreach (var point in GetComponentsInChildren<SkillSpawnPoint>(true))
            {
                if (!point.IsOccupied)
                    result.Add(point);
            }

            return result;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var point in GetComponentsInChildren<SkillSpawnPoint>(true))
            {
                Gizmos.color = point.IsOccupied ? Color.red : Color.green;
                Gizmos.DrawSphere(point.transform.position, 0.3f);
            }
        }
#endif
    }
}