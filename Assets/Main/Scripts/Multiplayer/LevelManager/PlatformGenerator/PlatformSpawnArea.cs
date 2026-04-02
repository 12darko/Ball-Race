using _Main.Scripts.Multiplayer.Multiplayer.SpawnController;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.LevelManager.PlatformGenerator
{
    public class PlatformSpawnArea : MonoBehaviour
    {
        [Header("Spawn Box (zorunlu)")] [SerializeField]
        private Collider spawnBox;

        [Header("Opsiyonel Spawn Points")] [SerializeField]
        private Transform[] spawnPoints;

        public Collider SpawnBox => spawnBox;
        public Transform[] SpawnPoints => spawnPoints;

        private void Reset()
        {
            if (spawnBox == null)
                spawnBox = GetComponentInChildren<SpawnerArea>().Collider;
        }
    }
}