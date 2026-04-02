using System;
using _Main.Scripts.Multiplayer.Multiplayer.InGame;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Scores;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Team;
using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer
{
    public class FinishLine : NetworkBehaviour
    {
  
        [UnitySerializeField, Networked] private NetworkString<_32> GameWinPlayerName { get; set; }
        [UnitySerializeField, Networked] private NetworkBool        ConfettiBool      { get; set; } = false;
        [UnitySerializeField, Networked] private int                GetTotalPoint     { get; set; }

        [UnitySerializeField, Networked] public NetworkString<_32> FirstPlayer  { get; set; }
        [UnitySerializeField, Networked] public NetworkString<_32> SecondPlayer { get; set; }
        [UnitySerializeField, Networked] public NetworkString<_32> ThirdPlayer  { get; set; }

        public event Action<int, string> OnPlayerFinished;

        // Inspector'dan bu finish hangi takıma ait → TeamA finish'e TeamB girerse gol
        [SerializeField] public TeamType BelongsToTeam = TeamType.None;

        [SerializeField] private int        minusTotalPoint;
        [SerializeField] private GameObject confettiObject;

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this, out _, out _))
            {
                if (change == nameof(ConfettiBool))
                    OnChangedConfettiBool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Runner.IsServer) return;
            if (!other.transform.parent.TryGetComponent(out NetworkObject networkObject)) return;

            var networkPlayers = networkObject.GetComponentInChildren<NetworkPlayers>();
            if (networkPlayers == null) return;
            if (networkPlayers.PlayerInFinish) return;

            var playerRef  = networkObject.InputAuthority;
            var playerName = networkPlayers.networkPlayerInGameData.PlayerName.Value;
            var status     = FindStatus(playerRef);

            // Soccer modu — karşı takım finish'ine girdi → gol
            if (IsSoccerGoal(status))
            {
                HandleSoccerGoal(playerRef, playerName, networkPlayers);
                return;
            }

            // Normal finish
            HandleNormalFinish(playerRef, playerName, networkPlayers, status);
        }

        // ----------------------------------------------------------------
        // Soccer
        // ----------------------------------------------------------------

        private bool IsSoccerGoal(NetworkPlayerStatus status)
        {
            if (BelongsToTeam == TeamType.None) return false;
            if (status == null) return false;

            // Oyuncunun takımı ile finish'in takımı farklıysa → gol
            return status.Team != TeamType.None && status.Team != BelongsToTeam;
        }

        private void HandleSoccerGoal(PlayerRef scorer, string scorerName,
                                       NetworkPlayers networkPlayers)
        {
            networkPlayers.PlayerInFinish = true;

            ScoringSystem.Instance?.AddGoalScore(scorer);

            var data = new EliminationData
            {
                Victim     = scorer,
                Killer     = scorer,
                Type       = EliminationType.Finished,
                VictimName = scorerName,
                KillerName = scorerName
            };

            NotificationManager.Instance?.AddNotificationItem(
                NotificationSelectType.Goal, data);
        }

        // ----------------------------------------------------------------
        // Normal finish (Standard / Lap / Team_Match)
        // ----------------------------------------------------------------

        private void HandleNormalFinish(PlayerRef playerRef, string playerName,
                                         NetworkPlayers networkPlayers,
                                         NetworkPlayerStatus status)
        {
            if (!ConfettiBool)
            {
                ConfettiBool      = true;
                GameWinPlayerName = networkPlayers.networkPlayerInGameData.PlayerName;
            }

            networkPlayers.PlayerInFinish = true;
            networkPlayers.networkPlayerInGameData.PlayerWinScore = GetTotalPoint;

            int position = SetPlayers(playerName);
            GetTotalPoint -= minusTotalPoint;

            if (status != null)
            {
                status.State          = NetworkPlayerState.Finished;
                status.FinishPosition = position;
            }

            ScoringSystem.Instance?.AddFinishScore(playerRef, position);
            SpectatorManager.Instance?.OnPlayerFinished(playerRef, position);
            OnPlayerFinished?.Invoke(position, playerName);
        }

        private void OnChangedConfettiBool()
        {
            confettiObject.SetActive(true);
        }

        private int SetPlayers(string playerName)
        {
            int position;

            if (string.IsNullOrEmpty(FirstPlayer.Value))
            {
                FirstPlayer = playerName;
                position    = 1;
            }
            else if (string.IsNullOrEmpty(SecondPlayer.Value))
            {
                SecondPlayer = playerName;
                position     = 2;
            }
            else if (string.IsNullOrEmpty(ThirdPlayer.Value))
            {
                ThirdPlayer = playerName;
                position    = 3;
            }
            else
            {
                position = 4;
            }

            NotificationManager.Instance?.AddNotificationItem(NotificationSelectType.Win);
            return position;
        }

        private NetworkPlayerStatus FindStatus(PlayerRef p)
        {
            foreach (var ps in FindObjectsOfType<NetworkPlayerStatus>())
                if (ps.Object.InputAuthority == p) return ps;
            return null;
        }
    }
}