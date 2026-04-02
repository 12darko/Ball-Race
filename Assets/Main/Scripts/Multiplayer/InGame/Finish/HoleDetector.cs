using _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Scores;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Team;
using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame
{
 public class HoleDetector : MonoBehaviour
    {
        // Soccer sahnesinde bu delik hangi takıma ait?
        // TeamA'nın deliğine TeamB girer → TeamB gol atar
        // Normal sahnelerde None bırak
        [SerializeField] public TeamType BelongsToTeam = TeamType.None;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.transform.parent.TryGetComponent<NetworkObject>(out var no)) return;
            if (!no.Runner.IsServer) return;

            var victim = no.InputAuthority;
            if (victim == PlayerRef.None) return;

            var networkPlayers = no.GetComponentInChildren<NetworkPlayers>();
            if (networkPlayers != null && networkPlayers.PlayerInFinish) return;

            var status = FindStatus(no, victim);

            // Soccer modu — karşı takımın deliğine girdi → gol
            if (IsSoccerGoal(status))
            {
                ScoringSystem.Instance?.AddGoalScore(victim);

                var goalData = new EliminationData
                {
                    Victim     = victim,
                    Killer     = victim,
                    Type       = EliminationType.Finished,
                    VictimName = status.DisplayName,
                    KillerName = status.DisplayName
                };

                NotificationManager.Instance?.AddNotificationItem(
                    NotificationSelectType.Goal, goalData);

                return;
            }

            // Normal eliminasyon
            var tracker = no.GetComponentInChildren<NetworkPlayerKnockbackTracker>();
            var killer  = tracker != null ? tracker.LastHitBy : PlayerRef.None;

            var type = killer != PlayerRef.None
                ? EliminationType.Pushed
                : EliminationType.FellAlone;

            NetworkPlayerEliminationHandler.Instance?.EliminatePlayer(victim, killer, type);
        }

        private bool IsSoccerGoal(NetworkPlayerStatus status)
        {
            if (BelongsToTeam == TeamType.None) return false;
            if (status == null) return false;
            return status.Team != TeamType.None && status.Team != BelongsToTeam;
        }

        private NetworkPlayerStatus FindStatus(NetworkObject no, PlayerRef p)
        {
            // Önce objenin üzerinde ara — daha hızlı
            var local = no.GetComponentInChildren<NetworkPlayerStatus>();
            if (local != null && local.Object.InputAuthority == p) return local;

            // Bulamazsan sahnede tara
            foreach (var ps in Object.FindObjectsOfType<NetworkPlayerStatus>())
                if (ps.Object.InputAuthority == p) return ps;

            return null;
        }
    }
}