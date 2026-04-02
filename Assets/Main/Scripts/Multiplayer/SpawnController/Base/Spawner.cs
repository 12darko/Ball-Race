using _Main.Scripts.Multiplayer.Mutlplayer.NetworkMisc;
using Fusion;
using Player.Runner;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.SpawnController
{
    public abstract class Spawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        public virtual void PlayerJoined(PlayerRef player)
        {
             
        }

        public virtual void PlayerLeft(PlayerRef player)
        {
             
        }

        public abstract void SetupGameVisualsForAllPlayers();
    }
}