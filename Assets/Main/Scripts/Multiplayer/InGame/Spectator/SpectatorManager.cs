using System.Collections.Generic;
using System.Linq;
using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator
{
  public class SpectatorManager : MonoBehaviour
    {
        public static SpectatorManager Instance { get; private set; }

        [SerializeField] private SpectatorCinemachine spectatorCamera;

        private List<NetworkPlayerStatus> _allPlayers = new();
        private NetworkPlayerStatus _localStatus;

        private void Awake() => Instance = this;

        private void Start() => RefreshPlayerList();

        public void EnterSpectatorMode(NetworkPlayerStatus localStatus)
        {
            _localStatus = localStatus;
            spectatorCamera.gameObject.SetActive(true);
            SpectatorUI.Instance?.gameObject.SetActive(true);

            if (localStatus.KilledBy != PlayerRef.None)
            {
                var killer = FindStatus(localStatus.KilledBy);
                if (killer != null && killer.State == NetworkPlayerState.Alive)
                {
                    Watch(killer);
                    return;
                }
            }

            WatchNextAlive(null);
        }

        public void OnPlayerFinished(PlayerRef player, int position)
        {
            if (_localStatus == null) return;
            if (_localStatus.Object.InputAuthority != player) return;

            spectatorCamera.gameObject.SetActive(true);
            SpectatorUI.Instance?.gameObject.SetActive(true);

            var next = _allPlayers
                .Where(p => p.State == NetworkPlayerState.Alive)
                .FirstOrDefault();

            if (next != null) Watch(next);
        }

        public void OnPlayerEliminated(EliminationData data) => RefreshPlayerList();

        public void WatchNextPlayer()
        {
            var alive   = Alive();
            if (alive.Count == 0) return;
            var current = _allPlayers.FirstOrDefault(
                p => p.transform == spectatorCamera.CurrentTarget);
            int idx = current != null ? (alive.IndexOf(current) + 1) % alive.Count : 0;
            Watch(alive[idx]);
        }

        public void WatchPreviousPlayer()
        {
            var alive   = Alive();
            if (alive.Count == 0) return;
            var current = _allPlayers.FirstOrDefault(
                p => p.transform == spectatorCamera.CurrentTarget);
            int idx = current != null
                ? (alive.IndexOf(current) - 1 + alive.Count) % alive.Count
                : 0;
            Watch(alive[idx]);
        }

        private void Watch(NetworkPlayerStatus target)
        {
            spectatorCamera.SetTarget(target.transform);
            SpectatorUI.Instance?.ShowTargetName(target.DisplayName.ToString());
        }

        private void WatchNextAlive(NetworkPlayerStatus current)
        {
            var alive = Alive();
            if (alive.Count == 0) return;
            int idx = current != null ? (alive.IndexOf(current) + 1) % alive.Count : 0;
            Watch(alive[idx]);
        }

        private List<NetworkPlayerStatus> Alive() =>
            _allPlayers.Where(p => p.State == NetworkPlayerState.Alive).ToList();

        private void RefreshPlayerList() =>
            _allPlayers = FindObjectsOfType<NetworkPlayerStatus>().ToList();

        private NetworkPlayerStatus FindStatus(PlayerRef p) =>
            _allPlayers.FirstOrDefault(ps => ps.Object.InputAuthority == p);
    }
}