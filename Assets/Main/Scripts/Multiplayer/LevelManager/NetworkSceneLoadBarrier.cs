using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.LevelManager
{
    public class NetworkSceneLoadBarrier : NetworkBehaviour
    {
        // Server tarafında loaded dediği bilinen oyuncular
        private static readonly HashSet<PlayerRef> LoadedPlayers = new HashSet<PlayerRef>();

        [Networked] private int ExpectedPlayerCount { get; set; }
        [Networked] private int LoadedCount { get; set; }
        [Networked] private NetworkBool GoSignal { get; set; }

        public override void Spawned()
        {
            // Her sahnede bu obje spawn/aktif olunca server state'i resetler
            if (Runner.IsServer)
            {
                LoadedPlayers.Clear();
                ExpectedPlayerCount = Runner.ActivePlayers.Count();
                LoadedCount = 0;
                GoSignal = false;

                Debug.Log($"[Barrier] Spawned. Expected={ExpectedPlayerCount}");
            }
        }

        /// <summary>
        /// Local client scene load bitince çağır.
        /// </summary>
        public void NotifyLocalLoaded()
        {
            // Local player kimse onu bildiriyoruz
            if (Runner == null) return;
            RPC_ClientLoaded(Runner.LocalPlayer);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_ClientLoaded(PlayerRef player)
        {
            if (!Runner.IsServer) return;

            // Beklenen sayıyı güncel tut (late join vs için)
            ExpectedPlayerCount = Runner.ActivePlayers.Count();

            if (LoadedPlayers.Contains(player))
                return;

            LoadedPlayers.Add(player);
            LoadedCount = LoadedPlayers.Count;

            Debug.Log($"[Barrier] Loaded: {player.PlayerId} ({LoadedCount}/{ExpectedPlayerCount})");

            if (LoadedCount >= ExpectedPlayerCount)
            {
                GoSignal = true;
                Debug.Log("[Barrier] GO!");
                RPC_Go();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_Go()
        {
            Debug.Log("[Barrier] GO received.");
            // İstersen burada input açma, UI kapama gibi işleri client tarafında da yaparsın.
        }

        public bool IsGo => GoSignal;
    }
}