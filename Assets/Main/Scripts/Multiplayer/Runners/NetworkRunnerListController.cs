using System;
using System.Collections.Generic;
using _Main.Scripts.Multiplayer.FusionCore.Lobby.Room;
using _Main.Scripts.Multiplayer.FusionCore.StartManager;
using _Main.Scripts.Multiplayer.Singleton;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace _Main.Scripts.Multiplayer.Runners
{
    public class NetworkRunnerListController : MonoBehaviour, INetworkRunnerCallbacks
    {
       
        [Header("Session Component")] [SerializeField]
        private NetworkRoomList networkRoomList;
        public NetworkRoomList NetworkRoomList => networkRoomList;

        private void Awake()
        {
            networkRoomList = FindObjectOfType<NetworkRoomList>(true);
        }
 
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }
        
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
   
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            if(networkRoomList == null)
                networkRoomList = FindObjectOfType<NetworkRoomList>(true);
            if (networkRoomList == null)
                return;
            
            if (sessionList.Count > 0)
            {
                networkRoomList.ClearList();

                foreach (var sessionInfo in sessionList)
                {
                    networkRoomList.AddToList(sessionInfo);
                }
            }
            else
            {
                Debug.Log("Session Not Found");
                networkRoomList.OnSessionNotFound();
            }
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            //Sahne Değişimi yaparken listi sıfırla
            networkRoomList = null;
        }
    }
}