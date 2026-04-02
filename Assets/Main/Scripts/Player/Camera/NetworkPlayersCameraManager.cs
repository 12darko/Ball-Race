using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Player
{
    public class NetworkPlayersCameraManager : NetworkBehaviour
    {
        public override void Spawned()
        {
            base.Spawned();
            if (NetworkPlayers.LocalPlayers.networkPlayersManager.LocalCamera.gameObject.activeSelf != true || NetworkPlayers.LocalPlayers.networkPlayersManager.LocalCinemachineCamera.gameObject.activeSelf != true) return;
            NetworkPlayers.LocalPlayers.networkPlayersManager.LocalCamera.transform.SetParent(null);
            NetworkPlayers.LocalPlayers.networkPlayersManager.LocalCinemachineCamera.transform.SetParent(null); 
        }

        private void Start()
        {
         
        }
    }
}