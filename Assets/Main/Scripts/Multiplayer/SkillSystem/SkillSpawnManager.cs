using Fusion;
using UnityEngine;
using System.Collections.Generic;

namespace Multiplayer.SkillSystem
{
    public class SkillSpawnManager : NetworkBehaviour
    {
        [Header("Skill Havuzu")]
        [SerializeField] private SkillData[] skillPool;
        public SkillData[] SkillPool => skillPool;

        [Header("Prefablar")]
        [SerializeField] private NetworkPrefabRef randomPickupPrefab;
        [SerializeField] private NetworkPrefabRef specificPickupPrefab;

        [Header("Platform başına kaç pickup?")]
        [SerializeField] private int minSpawnCount = 2;
        [SerializeField] private int maxSpawnCount = 5;

        [Header("Pickup yükseklik ofseti")]
        [SerializeField] private float heightOffset = 0.5f;

        [Header("Yüzey snap için Ground Layer")]
        [SerializeField] private LayerMask groundLayer;

        public void SpawnForPlatform(PlatformSkillSpawner platform)
        {
            if (!HasStateAuthority) return;
            if (skillPool == null || skillPool.Length == 0)
            {
                Debug.LogError("[SkillSpawnManager] skillPool boş!");
                return;
            }

            var points = platform.GetSpawnPoints();
            if (points.Count == 0)
            {
                Debug.LogWarning($"[SkillSpawnManager] {platform.name}: SkillSpawnPoint yok!");
                return;
            }

            int count = Mathf.Min(
                Random.Range(minSpawnCount, maxSpawnCount + 1),
                points.Count
            );

            Shuffle(points);

            for (int i = 0; i < count; i++)
            {
                var point = points[i];
                var capturedSkill = skillPool[Random.Range(0, skillPool.Length)];
                var capturedPoint = point;
                var spawnPos = GetSnappedPosition(point.transform.position);

                Runner.Spawn(
                    randomPickupPrefab,
                    spawnPos,
                    Quaternion.identity,
                    PlayerRef.None,
                    (runner, obj) =>
                    {
                        // onBeforeSpawned — Fusion register etmeden önce parent set et
                        obj.transform.SetParent(capturedPoint.transform, true);
                        obj.transform.position = spawnPos;

                        capturedPoint.IsOccupied = true;

                        if (obj.TryGetComponent(out SkillPickUpRandom rand))
                            rand.PickUpSkillData = capturedSkill;

                        Debug.Log($"[SkillSpawnManager] ✅ Spawn: {capturedPoint.name} → {spawnPos}");
                    }
                );
            }

            // Specific spawn'lar
            foreach (var entry in platform.SpecificSpawns)
            {
                if (entry.skill == null || entry.point == null) continue;
                if (!specificPickupPrefab.IsValid) continue;

                var capturedSkill = entry.skill;
                var capturedPoint = entry.point;
                var spawnPos = GetSnappedPosition(entry.point.transform.position);

                Runner.Spawn(
                    specificPickupPrefab,
                    spawnPos,
                    Quaternion.identity,
                    PlayerRef.None,
                    (runner, obj) =>
                    {
                        obj.transform.SetParent(capturedPoint.transform, true);
                        obj.transform.position = spawnPos;

                        capturedPoint.IsOccupied = true;

                        if (obj.TryGetComponent(out SkillPickUpSpecific specific))
                            specific.PickUpSkillData = capturedSkill;

                        Debug.Log($"[SkillSpawnManager] ✅ Specific Spawn → {spawnPos}");
                    }
                );
            }
        }

        private Vector3 GetSnappedPosition(Vector3 origin)
        {
            var rayOrigin = origin + Vector3.up * 2f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 10f, groundLayer))
                return hit.point + Vector3.up * heightOffset;

            Debug.LogWarning($"[SkillSpawnManager] Snap raycast tutmadı, fallback: {origin}");
            return origin + Vector3.up * heightOffset;
        }

        private void Shuffle(List<SkillSpawnPoint> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}