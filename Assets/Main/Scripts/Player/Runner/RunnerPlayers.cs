using Fusion;
using UnityEngine;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Multiplayer.Player;

namespace Player.Runner
{
    public class RunnerPlayers : NetworkBehaviour, IPlayerLeft
    {
        [Header("Owner")]
        [Networked] public PlayerRef Owner { get; private set; }

        [Header("Category")]
        [Networked] public GameModeCategory CurrentCategory { get; set; }

        [Header("Cosmetic Indexes")]
        [Networked] public int BallIndex { get; set; }
        [Networked] public int HatIndex { get; set; }
        [Networked] public int FaceIndex { get; set; }
        
        [Header("Player Info")]
        [Networked] public NetworkString<_32> PlayerName { get; set; }
        
        [Header("Lobby Ready System")]
        [Networked] public NetworkBool IsReady { get; set; }
        
        [Header("Index Ready Flag")]
        [Networked] public NetworkBool IndexesReady { get; set; }
        
        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
                Owner = Object.InputAuthority;

            RunnerPlayersRegistry.Register(Object.InputAuthority, this);

            if (Runner.SessionInfo.Properties.TryGetValue("GameCategory", out var categoryVal))
                CurrentCategory = (GameModeCategory)(int)categoryVal;

            // ✅ Local player ise index'leri HEMEN gönder
            if (Object.HasInputAuthority)
            {
                if (PlayerData.Instance != null)
                {
                    Debug.Log($"[RunnerPlayers] Sending player data for {Object.InputAuthority}");
                    RPC_SetPlayerData(
                        PlayerData.Instance.PlayerName,
                        PlayerData.Instance.SelectedBallIndex,
                        PlayerData.Instance.SelectedHatIndex,
                        PlayerData.Instance.SelectedFaceIndex
                    );
                }
                else
                {
                    Debug.LogError("[RunnerPlayers] PlayerData.Instance is NULL!");
                }
            }
            else
            {
                Debug.Log($"[RunnerPlayers] Remote player spawned: {Object.InputAuthority}");
            }
            
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            if (Lobby.LobbyManager.Instance != null)
            {
                Lobby.LobbyManager.Instance.OnPlayerJoined(this);
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            RunnerPlayersRegistry.Unregister(Object.InputAuthority, this);
            
            if (Lobby.LobbyManager.Instance != null)
            {
                Lobby.LobbyManager.Instance.OnPlayerLeft(this);
            }
        }
        
        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(IsReady):
                    case nameof(PlayerName):
                        if (Lobby.LobbyManager.Instance != null)
                        {
                            Lobby.LobbyManager.Instance.OnPlayerDataChanged(this);
                        }
                        break;
                    
                    case nameof(IndexesReady):
                        if (IndexesReady)
                        {
                            Debug.Log($"[RunnerPlayers] IndexesReady changed to true for {Owner}");
                        }
                        break;
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetPlayerData(NetworkString<_32> name, int ball, int hat, int face)
        {
            PlayerName = name;
            BallIndex = ball;
            HatIndex = hat;
            FaceIndex = face;
            IndexesReady = true;
            
            Debug.Log($"[RunnerPlayers] ✅ Data received on server: {name}, Ball={ball}, Hat={hat}, Face={face}");
        }

        public void PushFromPlayerData()
        {
            if (!Object.HasInputAuthority) return;

            var pd = PlayerData.Instance;
            if (pd == null) return;

            RPC_SetIndexes(pd.SelectedBallIndex, pd.SelectedHatIndex, pd.SelectedFaceIndex);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetIndexes(int ball, int hat, int face)
        {
            BallIndex = ball;
            HatIndex = hat;
            FaceIndex = face;
            IndexesReady = true;
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetReady(bool isReady)
        {
            IsReady = isReady;
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (!Object.HasStateAuthority) return;

            if (player == Owner)
            {
                Runner.Despawn(Object);
            }
        }
    }
}