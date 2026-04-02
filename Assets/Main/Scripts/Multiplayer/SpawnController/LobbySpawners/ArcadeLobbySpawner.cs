using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Multiplayer.Multiplayer.Customize;
using _Main.Scripts.Multiplayer.SpawnController;
using Fusion;
using Main.Scripts.Player.Database;
using Multiplayer.Player;
using Player.Runner;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.SpawnController
{
    public class ArcadeLobbySpawner : Spawner
    {
        [Header("DB")]
        [SerializeField] private CosmeticDatabase cosmeticDB;

        [Header("Prefabs")]
        [SerializeField] private NetworkObject lobbyBoatPrefab;
        [SerializeField] private NetworkObject lobbyPreviewRootPrefab;

        [Header("Spawn Points")]
        [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

        [Header("Target Points")]
        [SerializeField] private List<Transform> targetPoints = new List<Transform>();
        [SerializeField] private bool rotateToTarget = true;

        [Header("Seat Settings")]
        [SerializeField] private string previewSeatName = "SeatPoint";

        private readonly Queue<int> _freeSlots = new Queue<int>();
        private readonly Dictionary<PlayerRef, int> _playerToSlot = new Dictionary<PlayerRef, int>();
        private bool _inited;

        private readonly Dictionary<PlayerRef, NetworkObject> _boats = new Dictionary<PlayerRef, NetworkObject>();
        private readonly Dictionary<PlayerRef, NetworkObject> _roots = new Dictionary<PlayerRef, NetworkObject>();
        private readonly Dictionary<PlayerRef, NetworkObject> _balls = new Dictionary<PlayerRef, NetworkObject>();
        private readonly Dictionary<PlayerRef, NetworkObject> _hats = new Dictionary<PlayerRef, NetworkObject>();
        private readonly Dictionary<PlayerRef, NetworkObject> _faces = new Dictionary<PlayerRef, NetworkObject>();

        private void EnsureInit()
        {
            if (_inited) return;

            _freeSlots.Clear();
            _playerToSlot.Clear();

            for (int i = 0; i < spawnPoints.Count; i++)
                _freeSlots.Enqueue(i);

            _inited = true;
        }

        public override void PlayerJoined(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            Debug.Log($"[ArcadeLobbySpawner] PlayerJoined: {player}");

            EnsureInit();

            if (_playerToSlot.ContainsKey(player))
                return;

            if (_freeSlots.Count == 0)
            {
                Debug.LogError("[ArcadeLobbySpawner] Boş slot yok!");
                return;
            }

            int slot = _freeSlots.Dequeue();
            _playerToSlot[player] = slot;

            Transform point = spawnPoints[slot];
            if (point == null || lobbyBoatPrefab == null || lobbyPreviewRootPrefab == null)
            {
                ReturnSlot(player);
                Debug.LogError("[ArcadeLobbySpawner] Prefab veya spawn point null!");
                return;
            }

            // ✅ Biraz bekle - RunnerPlayers RPC'sinin gelmesini bekle
            StartCoroutine(DelayedSpawn(player, slot, point));
        }

        private IEnumerator DelayedSpawn(PlayerRef player, int slot, Transform spawnPoint)
        {
            // 0.5 saniye bekle - RunnerPlayers'ın spawn olup RPC almasını bekle
            yield return new WaitForSeconds(0.5f);
    
            Debug.Log($"[ArcadeLobbySpawner] Starting delayed spawn check for {player}");
    
            yield return StartCoroutine(WaitForRunnerPlayerThenSpawn(player, slot, spawnPoint));
        }

        private IEnumerator WaitForRunnerPlayerThenSpawn(PlayerRef player, int slot, Transform spawnPoint)
        {
           RunnerPlayers runnerPlayer = null;
            int attempts = 0;
    
            Debug.Log($"[ArcadeLobbySpawner] ⏳ Waiting for RunnerPlayers and IndexesReady for {player}...");
    
            // ✅ İlk önce RunnerPlayers'ın spawn olmasını bekle
            while (attempts < 180) // 9 saniye max (daha uzun)
            {
                runnerPlayer = RunnerPlayersRegistry.Get(player);
        
                if (runnerPlayer != null)
                {
                    Debug.Log($"[ArcadeLobbySpawner] ✅ RunnerPlayers found for {player}, IndexesReady={runnerPlayer.IndexesReady}");
            
                    if (runnerPlayer.IndexesReady)
                    {
                        Debug.Log($"[ArcadeLobbySpawner] ✅ IndexesReady is TRUE for {player}");
                        break;
                    }
                    else
                    {
                        Debug.Log($"[ArcadeLobbySpawner] ⏳ Waiting for IndexesReady... (attempt {attempts})");
                    }
                }
                else
                {
                    Debug.Log($"[ArcadeLobbySpawner] ⏳ RunnerPlayers not found yet... (attempt {attempts})");
                }
            
                attempts++;
                yield return null;
            }

            if (runnerPlayer == null || !runnerPlayer.IndexesReady)
            {
                Debug.LogError($"[ArcadeLobbySpawner] ❌ RunnerPlayers timeout: {player} (attempts: {attempts}, exists: {runnerPlayer != null}, ready: {runnerPlayer?.IndexesReady})");
                ReturnSlot(player);
                yield break;
            }

            // Index'leri al
            int ballIndex = runnerPlayer.BallIndex;
            int hatIndex = runnerPlayer.HatIndex;
            int faceIndex = runnerPlayer.FaceIndex;

            Debug.Log($"[ArcadeLobbySpawner] 🚀 Starting spawn for {player}: Ball={ballIndex}, Hat={hatIndex}, Face={faceIndex}");

            // Spawn işlemini başlat
            yield return StartCoroutine(SpawnPlayerSetup(player, slot, spawnPoint, ballIndex, hatIndex, faceIndex));
        }

        private IEnumerator SpawnPlayerSetup(PlayerRef player, int slot, Transform spawnPoint, int ballIndex, int hatIndex, int faceIndex)
        {
            Debug.Log($"[ArcadeLobbySpawner] ===== SPAWNING PLAYER {player} =====");
            Debug.Log($"[ArcadeLobbySpawner] Indexes: Ball={ballIndex}, Hat={hatIndex}, Face={faceIndex}");

            // ✅ 1) BOAT spawn
            var boat = Runner.Spawn(lobbyBoatPrefab, spawnPoint.position, spawnPoint.rotation, player);
            _boats[player] = boat;
            Debug.Log($"[ArcadeLobbySpawner] ✅ Boat spawned: {boat.name}");
            
            yield return null;

            // ✅ 2) Boat'ı hedefe gönder
            SendBoatToTarget(slot, boat);

            // ✅ 3) Seat bul
            Transform seat = FindDeepChild(boat.transform, previewSeatName);
            if (seat == null)
            {
                Debug.LogError($"[ArcadeLobbySpawner] '{previewSeatName}' bulunamadı!");
                yield break;
            }

            // ✅ 4) Preview Root spawn
            var root = Runner.Spawn(lobbyPreviewRootPrefab, seat.position, seat.rotation, player);
            _roots[player] = root;
            Debug.Log($"[ArcadeLobbySpawner] ✅ Root spawned: {root.name}");
            
            yield return null;

            // ✅ 5) Root'a parent bilgisini ver
            var rootParenter = root.GetComponent<LobbyRootParenter>();
            if (rootParenter != null)
            {
                rootParenter.SetSeatParent(boat.Id);
            }

            // ✅ 6) BallRoot bul
            Transform ballRoot = FindDeepChild(root.transform, "BallRoot");
            if (ballRoot == null)
            {
                Debug.LogError("[ArcadeLobbySpawner] 'BallRoot' bulunamadı!");
                yield break;
            }

            // ✅ 7) BALL spawn
            if (cosmeticDB == null || cosmeticDB.allBalls == null || cosmeticDB.allBalls.Count == 0)
            {
                Debug.LogError("[ArcadeLobbySpawner] cosmeticDB boş!");
                yield break;
            }

            if (ballIndex < 0 || ballIndex >= cosmeticDB.allBalls.Count)
            {
                Debug.LogWarning($"[ArcadeLobbySpawner] Ball index out of range: {ballIndex}, using 0");
                ballIndex = 0;
            }

            var ballItem = cosmeticDB.allBalls[ballIndex];
            Debug.Log($"[ArcadeLobbySpawner] Ball item: {ballItem.name}");

            if (!ballItem.CustomizeItemPrefabRefLobby.IsValid)
            {
                Debug.LogError($"[ArcadeLobbySpawner] Ball PrefabRef INVALID! index={ballIndex}");
                yield break;
            }

            var ball = Runner.Spawn(ballItem.CustomizeItemPrefabRefLobby, ballRoot.position, ballRoot.rotation, player);
            _balls[player] = ball;
            Debug.Log($"[ArcadeLobbySpawner] ✅ Ball spawned: {ball.name}");
            
            yield return null;

            // ✅ 8) HAT spawn
            NetworkObject hatObj = null;
            float hatLocalY = 0f;

            if (cosmeticDB.allHats != null && cosmeticDB.allHats.Count > 0)
            {
                if (hatIndex < 0 || hatIndex >= cosmeticDB.allHats.Count)
                {
                    Debug.LogWarning($"[ArcadeLobbySpawner] Hat index out of range: {hatIndex}, using 0");
                    hatIndex = 0;
                }

                var hatItem = cosmeticDB.allHats[hatIndex];
                hatLocalY = hatItem.CustomizeInGameSpawnYOffset;
                Debug.Log($"[ArcadeLobbySpawner] Hat item: {hatItem.name}, Y offset: {hatLocalY}");

                var hatRef = hatItem.CustomizeItemPrefabRefLobby.IsValid
                    ? hatItem.CustomizeItemPrefabRefLobby
                    : hatItem.CustomizeItemPrefabRef;

                if (hatRef.IsValid)
                {
                    hatObj = Runner.Spawn(hatRef, ball.transform.position, Quaternion.identity, player);
                    _hats[player] = hatObj;
                    Debug.Log($"[ArcadeLobbySpawner] ✅ Hat spawned: {hatObj.name}");
                    yield return null;
                }
                else
                {
                    Debug.LogWarning($"[ArcadeLobbySpawner] Hat PrefabRef INVALID!");
                }
            }

            // ✅ 9) FACE spawn
            NetworkObject faceObj = null;
            if (cosmeticDB.allFaces != null && cosmeticDB.allFaces.Count > 0)
            {
                if (faceIndex < 0 || faceIndex >= cosmeticDB.allFaces.Count)
                {
                    Debug.LogWarning($"[ArcadeLobbySpawner] Face index out of range: {faceIndex}, using 0");
                    faceIndex = 0;
                }

                var faceItem = cosmeticDB.allFaces[faceIndex];
                Debug.Log($"[ArcadeLobbySpawner] Face item: {faceItem.name}");

                var faceRef = faceItem.CustomizeItemPrefabRefLobby.IsValid
                    ? faceItem.CustomizeItemPrefabRefLobby
                    : faceItem.CustomizeItemPrefabRef;

                if (faceRef.IsValid)
                {
                    faceObj = Runner.Spawn(faceRef, ball.transform.position, Quaternion.identity, player);
                    _faces[player] = faceObj;
                    Debug.Log($"[ArcadeLobbySpawner] ✅ Face spawned: {faceObj.name}");
                    yield return null;
                }
                else
                {
                    Debug.LogWarning($"[ArcadeLobbySpawner] Face PrefabRef INVALID!");
                }
            }

            // ✅ 10) Cosmetics'i bind et
            var binder = ball.GetComponent<LobbyCosmeticBinder>();
            if (binder != null)
            {
                binder.SetCosmetics(hatObj, hatIndex, faceObj, faceIndex, hatLocalY);
                Debug.Log($"[ArcadeLobbySpawner] ✅ Cosmetics bound to Ball");
            }
            
            Debug.Log($"[ArcadeLobbySpawner] ===== SPAWN COMPLETE {player} =====");
        }

        private void SendBoatToTarget(int slot, NetworkObject boat)
        {
            if (boat == null || targetPoints == null || targetPoints.Count == 0) return;
            if (slot < 0 || slot >= targetPoints.Count || targetPoints[slot] == null) return;

            var mover = boat.GetComponent<LobbyBoatMover>();
            if (mover == null) return;

            var target = targetPoints[slot];

            if (rotateToTarget)
                mover.MoveTo(target.position, target.rotation);
            else
                mover.MoveTo(target.position);
        }

        public override void PlayerLeft(PlayerRef player)
        {
            if (!Runner.IsServer) return;
            
            DespawnAll(player);
            ReturnSlot(player);
        }

        public override void SetupGameVisualsForAllPlayers() { }

        private void DespawnAll(PlayerRef player)
        {
            if (Runner == null || !Runner.IsRunning) return;
            
            try
            {
                if (_faces.TryGetValue(player, out var f) && f != null && f.IsValid)
                {
                    Runner.Despawn(f);
                }
                
                if (_hats.TryGetValue(player, out var h) && h != null && h.IsValid)
                {
                    Runner.Despawn(h);
                }
                
                if (_balls.TryGetValue(player, out var b) && b != null && b.IsValid)
                {
                    Runner.Despawn(b);
                }
                
                if (_roots.TryGetValue(player, out var r) && r != null && r.IsValid)
                {
                    Runner.Despawn(r);
                }
                
                if (_boats.TryGetValue(player, out var boat) && boat != null && boat.IsValid)
                {
                    Runner.Despawn(boat);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ArcadeLobbySpawner] Despawn error: {e.Message}");
            }
            finally
            {
                _faces.Remove(player);
                _hats.Remove(player);
                _balls.Remove(player);
                _roots.Remove(player);
                _boats.Remove(player);
            }
        }

        // ✅ Slot'u geri iade et
        private void ReturnSlot(PlayerRef player)
        {
            if (!_playerToSlot.TryGetValue(player, out int slot)) return;
            _playerToSlot.Remove(player);
            _freeSlots.Enqueue(slot);
        }

        private Transform FindDeepChild(Transform parent, string name)
        {
            if (parent == null) return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i);
                if (c.name == name) return c;

                var r = FindDeepChild(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}