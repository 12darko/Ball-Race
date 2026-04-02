using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Multiplayer.SpawnController;
using Fusion;
using Main.Scripts.Player;
using Main.Scripts.Player.Database;
using Player;
using Player.Runner;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.SpawnController
{
    public class StandardSpawner : Spawner
    {
        [Header("Platform / Spawn Area")]
        [SerializeField] private NetworkPlatformGenerator platformGenerator;
        [SerializeField] private float playerSpacing = 1.5f;

        [Tooltip("Platform spawn + ActiveArea bulunması için max frame bekleme")]
        [SerializeField] private int platformWaitFrames = 60;

        [Header("DB")]
        [SerializeField] private CosmeticDatabase cosmeticDB;

        [Header("Fallback (DB boşsa)")]
        [SerializeField] private NetworkPrefabRef defaultGamePrefab;

        [Header("RunnerPlayers Fallback (Game Scene)")]
        [Tooltip("Game sahnesinde RunnerPlayers yoksa server bunu spawn eder. (NetworkObject + RunnerPlayers)")]
        [SerializeField] private NetworkObject runnerPlayersPrefab;

        [Tooltip("Registry dolması için kaç frame bekleyelim?")]
        [SerializeField] private int retryFrames = 15;

        [Header("Open Side Facing (opsiyonel)")]
        [Tooltip("Açık tarafa baktırma hesabını yapan helper. Boş bırakırsan Quaternion.identity spawn eder.")]
        [SerializeField] private SpawnersOpenSideFacingHelper openSideFacing;

        private readonly Dictionary<PlayerRef, NetworkObject> _gameModels = new();
        private Coroutine _setupRoutine;
        private bool _setupStarted;
        private bool _modelsSpawned;

        // ✅ DIŞARIDAN ÇAĞIR
        public override void SetupGameVisualsForAllPlayers()
        {
            var runner = FindObjectOfType<NetworkRunner>();
            if (runner == null || !runner.IsServer) return;

            if (_setupStarted)
            {
                Debug.Log("[StandardSpawner] Setup zaten başladı, tekrar çalıştırmadım.");
                return;
            }

            _setupStarted = true;

            if (_setupRoutine != null)
                StopCoroutine(_setupRoutine);

            _setupRoutine = StartCoroutine(SetupRoutine());
        }


        private IEnumerator SetupRoutine()
        {
            var runner = FindObjectOfType<NetworkRunner>();
            if (runner == null || !runner.IsServer) yield break;

            // ✅ 0) ÖNCE PLATFORM SPAWN (SERVER)
            yield return StartCoroutine(Server_SpawnPlatformAndWaitReady());

            // ✅ 1) RunnerPlayers yoksa spawn et
           EnsureRunnerPlayersForAllPlayers(runner);

            // ✅ 2) Registry dolsun diye bekle
            for (int i = 0; i < retryFrames; i++)
            {
                if (IsRunnerPlayersReady(runner))
                    break;

                yield return null;
            }

            // ✅ 3) Game modelleri spawnla
            SetupOnce(runner);

            _setupRoutine = null;
        }

        private IEnumerator Server_SpawnPlatformAndWaitReady()
        {
            if (platformGenerator == null)
            {
                Debug.LogWarning("[StandardSpawner] platformGenerator yok! Platform spawn edilmeyecek.");
                yield break;
            }

            // platformu server spawn et
            platformGenerator.Server_SpawnRandomPlatform();

            // ActiveArea + SpawnBox hazır olana kadar bekle
            for (int f = 0; f < platformWaitFrames; f++)
            {
                if (platformGenerator.ActiveArea != null && platformGenerator.ActiveArea.SpawnBox != null)
                    break;

                yield return null;
            }

            if (platformGenerator.ActiveArea == null || platformGenerator.ActiveArea.SpawnBox == null)
            {
                Debug.LogWarning("[StandardSpawner] Platform spawnlandı ama ActiveArea/SpawnBox hazır değil! Spawnlar Vector3.zero'a düşebilir.");
                yield break;
            }

            // ✅ ÖNEMLİ: Scale + collider bounds + physics otursun
            yield return null;                     // 1 frame
            Physics.SyncTransforms();              // transform->physics sync
            yield return new WaitForFixedUpdate(); // physics step
        }

        private void EnsureRunnerPlayersForAllPlayers(NetworkRunner runner)
        {
            if (runnerPlayersPrefab == null)
            {
                Debug.LogWarning("[StandardSpawner] runnerPlayersPrefab atanmadı! RunnerPlayers spawn edemem.");
                return;
            }

            foreach (var p in runner.ActivePlayers)
            {
                if (RunnerPlayersRegistry.Get(p) != null)
                    continue;

                runner.Spawn(runnerPlayersPrefab, Vector3.zero, Quaternion.identity, p);
                Debug.Log($"[StandardSpawner] RunnerPlayers yoktu, spawn edildi. Player={p}");
            }
        }

        private bool IsRunnerPlayersReady(NetworkRunner runner)
        {
            foreach (var p in runner.ActivePlayers)
            {
                if (RunnerPlayersRegistry.Get(p) == null)
                    return false;
            }
            return true;
        }

        private void SetupOnce(NetworkRunner runner)
        {
            if (_modelsSpawned) return;
            _modelsSpawned = true;

            int i = 0;
            foreach (var player in runner.ActivePlayers)
            {
                var rp = RunnerPlayersRegistry.Get(player);
                if (rp == null) continue;

                Vector3 pos = CalculateLinearPosition(i++);
                SpawnOrRespawnGameModel(runner, player, pos, rp.BallIndex, rp.HatIndex, rp.FaceIndex);
            }

            Debug.Log("[StandardSpawner] Game modeller spawnlandı (tek sefer).");
        }


        private void SpawnOrRespawnGameModel(NetworkRunner runner, PlayerRef player, Vector3 pos, int ballIndex, int hatIndex, int faceIndex)
        {
            if (_gameModels.TryGetValue(player, out var old) && old != null)
                runner.Despawn(old);

            _gameModels.Remove(player);

            NetworkPrefabRef ballRef = defaultGamePrefab;

            if (cosmeticDB != null && cosmeticDB.allBalls != null && cosmeticDB.allBalls.Count > 0)
            {
                ballIndex = Mathf.Clamp(ballIndex, 0, cosmeticDB.allBalls.Count - 1);
                var ballItem = cosmeticDB.allBalls[ballIndex];

                if (ballItem.CustomizeItemPrefabRef.IsValid)
                    ballRef = ballItem.CustomizeItemPrefabRef;
            }

            if (!ballRef.IsValid)
            {
                Debug.LogError("[StandardSpawner] Game model prefab invalid (ne DB ne fallback)!");
                return;
            }

            // ✅ Rotasyon: açık tarafa baksın (server hesaplar -> Fusion replicate eder)
            Quaternion rot = Quaternion.identity;

            if (openSideFacing != null)
            {
                Transform spawnBoxT = platformGenerator != null &&
                                      platformGenerator.ActiveArea != null &&
                                      platformGenerator.ActiveArea.SpawnBox != null
                    ? platformGenerator.ActiveArea.SpawnBox.transform
                    : null;

                rot = openSideFacing.CalculateOpenFacing(pos, spawnBoxT);
            }

            var modelNO = runner.Spawn(ballRef, pos, rot, player);
            _gameModels[player] = modelNO;

            var ingameData = modelNO.GetComponentInChildren<NetworkPlayerInGameData>();
            if (ingameData == null)
            {
                Debug.LogError("[StandardSpawner] Spawnlanan game model prefabında NetworkPlayerInGameData yok!");
                return;
            }

            ingameData.PlayerId = player.PlayerId;
            ingameData.NetworkedHatIndex = hatIndex;
            ingameData.NetworkedFaceIndex = faceIndex;
        }

        private Vector3 CalculateLinearPosition(int index)
        {
            if (platformGenerator == null || platformGenerator.ActiveArea == null || platformGenerator.ActiveArea.SpawnBox == null)
            {
                Debug.LogWarning("[StandardSpawner] ActiveArea/SpawnBox yok! Vector3.zero döndüm.");
                return Vector3.zero;
            }

            var spawnBox = platformGenerator.ActiveArea.SpawnBox;
            Bounds b = spawnBox.bounds;

            // Box'ın sağ eksenine göre sol baştan sırala
            Vector3 startPoint = b.center - (spawnBox.transform.right * b.extents.x);

            float offset = (index * playerSpacing) + (playerSpacing * 0.5f);
            Vector3 p = startPoint + (spawnBox.transform.right * offset);

            // ✅ Raycast: trigger’ları IGNORE et (SpawnBox trigger olsa bile)
            Vector3 rayStart = new Vector3(p.x, b.max.y + 10f, p.z);

            if (Physics.Raycast(rayStart, Vector3.down, out var hit, 200f, ~0, QueryTriggerInteraction.Ignore))
            {
                p.y = hit.point.y + 0.05f; // biraz havada dursun
            }
            else
            {
                // hit yoksa en azından platform bounds üstüne koy
                p.y = b.max.y + 0.1f;
                Debug.LogWarning("[StandardSpawner] Raycast hit yok. Layer/Collider yok olabilir. Bounds üstüne koydum.");
            }

            return p;
        }
        
        private void OnDisable()
        {
            _setupStarted = false;
            _modelsSpawned = false;
            _gameModels.Clear();
        }
    }
}