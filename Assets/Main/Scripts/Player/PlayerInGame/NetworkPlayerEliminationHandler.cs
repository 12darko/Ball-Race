using _Main.Scripts.Multiplayer.Multiplayer.InGame;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Scores;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator;
using Fusion;
using UnityEngine;

namespace Player
{
    public class NetworkPlayerEliminationHandler : NetworkBehaviour
    {
        public static NetworkPlayerEliminationHandler Instance { get; private set; }

        [SerializeField] private ScoringSystem scoringSystem;

        private void Awake() => Instance = this;

        public void EliminatePlayer(PlayerRef victim, PlayerRef killer, EliminationType type)
        {
            if (!Runner.IsServer) return;

            var victimStatus = FindStatus(victim);
            if (victimStatus == null || victimStatus.State != NetworkPlayerState.Alive) return;

            victimStatus.State    = NetworkPlayerState.Spectating;
            victimStatus.KilledBy = killer;

            var data = new EliminationData
            {
                Victim     = victim,
                Killer     = killer,
                Type       = type,
                VictimName = victimStatus.DisplayName,
                KillerName = killer != PlayerRef.None
                    ? (FindStatus(killer)?.DisplayName ?? default)
                    : default
            };

            if (type == EliminationType.Pushed && killer != PlayerRef.None)
                scoringSystem?.AddKillScore(killer);

            var notifType = type == EliminationType.Pushed
                ? NotificationSelectType.KillFeed
                : NotificationSelectType.Falling;

            NotificationManager.Instance?.AddNotificationItem(notifType, data);

            RPC_NotifyElimination(data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_NotifyElimination(EliminationData data)
        {
            SpectatorManager.Instance?.OnPlayerEliminated(data);
        }

        private NetworkPlayerStatus FindStatus(PlayerRef playerRef)
        {
            foreach (var ps in FindObjectsOfType<NetworkPlayerStatus>())
                if (ps.Object.InputAuthority == playerRef) return ps;
            return null;
        }
    }
}