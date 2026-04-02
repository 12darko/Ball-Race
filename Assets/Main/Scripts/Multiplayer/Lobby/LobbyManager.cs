using System.Collections.Generic;
using Fusion;
using Lobby.UI;
using Player.Runner;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        public static LobbyManager Instance { get; private set; }
        
        [Header("References")]
        [SerializeField] private LobbyUIManager uiManager;
        
        [Header("Lobby Settings")]
        [SerializeField] private int minPlayersToStart = 2;
        [SerializeField] private float countdownDuration = 5f;
        
        [Header("Lobby Info")]
        [SerializeField] private string lobbyName = "Lobby Name";
        [SerializeField] private string gameMode = "Game Mode";
        [SerializeField] private string gameMap = "Game Map";
        
        [Networked] private NetworkBool AllPlayersReady { get; set; }
        [Networked] private float CountdownTimer { get; set; }
        [Networked] private NetworkBool IsCountingDown { get; set; }
        
        private Dictionary<PlayerRef, RunnerPlayers> _playersMap = new Dictionary<PlayerRef, RunnerPlayers>();
         
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        public override void Spawned()
        {
     
            if (uiManager != null)
                uiManager.Initialize(this);
    
            // ✅ Session properties'den lobby bilgilerini al
            if (Runner.SessionInfo.Properties.TryGetValue("LobbyName", out var lobbyNameProp))
                lobbyName = (string)lobbyNameProp.PropertyValue.ToString();
    
            if (Runner.SessionInfo.Properties.TryGetValue("GameMode", out var gameModeProp))
                gameMode = gameModeProp.PropertyValue.ToString();
    
            if (Runner.SessionInfo.Properties.TryGetValue("GameMap", out var gameMapProp))
                gameMap = gameMapProp.PropertyValue.ToString();
    
            UpdateLobbyUI();
    
            // Mevcut oyuncuları al
            foreach (var player in Runner.ActivePlayers)
            {
                var runnerPlayer = RunnerPlayersRegistry.Get(player);
                if (runnerPlayer != null)
                {
                    OnPlayerJoined(runnerPlayer);
                }
            }
        }
        
        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;
            
            CheckAllPlayersReady();
            
            if (IsCountingDown)
            {
                CountdownTimer -= Runner.DeltaTime;
                
                if (CountdownTimer <= 0)
                {
                    StartGame();
                }
            }
        }
        
        public override void Render()
        {
            if (IsCountingDown)
            {
                int seconds = Mathf.CeilToInt(CountdownTimer);
                uiManager?.ShowCountdown(seconds);
            }
            else
            {
                uiManager?.HideCountdown();
            }
        }
        
        private void CheckAllPlayersReady()
        {
            if (!Object.HasStateAuthority) 
                return;

            int readyPlayerCount = 0;
            int totalValidPlayers = 0;

            foreach (var playerRef in Runner.ActivePlayers)
            {
                var runnerPlayer = RunnerPlayersRegistry.Get(playerRef);

                if (runnerPlayer == null)
                    continue; // henüz spawn olmamış

                totalValidPlayers++;

                if (runnerPlayer.IsReady)
                    readyPlayerCount++;
            }

            if (totalValidPlayers < minPlayersToStart)
            {
                IsCountingDown = false;
                return;
            }

            if (readyPlayerCount == totalValidPlayers)
            {
                if (!IsCountingDown)
                {
                    IsCountingDown = true;
                    CountdownTimer = countdownDuration;
                }
            }
            else
            {
                IsCountingDown = false;
            }
        }

        
        // ✅ RunnerPlayers callback'leri
        public void OnPlayerJoined(RunnerPlayers player)
        {
            _playersMap[player.Owner] = player;
            UpdatePlayerUI(player);
            UpdateLobbyUI();
        }
        
        public void OnPlayerLeft(RunnerPlayers player)
        {
            _playersMap.Remove(player.Owner);
           // uiManager?.RemovePlayer(player.Owner);
            UpdateLobbyUI();
        }
        
        public void OnPlayerDataChanged(RunnerPlayers player)
        {
            UpdatePlayerUI(player);
            
            // Local player için ready butonunu güncelle
            if (player.Object.HasInputAuthority)
            {
                uiManager?.UpdateReadyButton(player.IsReady);
            }
        }
        
        private void UpdatePlayerUI(RunnerPlayers player)
        {
        /*    uiManager?.AddPlayer(
                player.Owner,
                player.PlayerName.ToString(),
                player.IsReady
            );*/
        }
        
        private void UpdateLobbyUI()
        {
            int maxPlayers = Runner != null ? Runner.SessionInfo.MaxPlayers : 8;
            
            uiManager?.UpdateLobbyInfo(
                lobbyName,
                _playersMap.Count,
                maxPlayers,
                gameMode,
                gameMap
            );
        }
        
        public void SetLocalPlayerReady(bool isReady)
        {
            // ✅ Local RunnerPlayers'ı bul ve ready set et
            foreach (var kvp in _playersMap)
            {
                if (kvp.Value.Object.HasInputAuthority)
                {
                    kvp.Value.RPC_SetReady(isReady);
                    break;
                }
            }
        }
        
        private void StartGame()
        {
            Debug.Log("[LobbyManager] Starting game!");
            
            // Oyun scene'ine geç
            if (Object.HasStateAuthority)
            {
                // TODO: Game scene'ine geçiş
                Runner.LoadScene("Map Autumn");
            }
        }
        
        public void LeaveLobby()
        {
            if (Runner != null)
            {
                Runner.Shutdown();
            }
            
            // Ana menüye dön
            SceneManager.LoadScene("MainMenu");
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}