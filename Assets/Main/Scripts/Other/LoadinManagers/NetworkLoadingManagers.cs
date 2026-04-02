using System.Collections.Generic;
using Fusion;
using Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Other.LoadinManagers
{
    public class NetworkLoadingManagers : NetworkBehaviour
    {
        public static NetworkLoadingManagers Instance;
        private Dictionary<ulong, float> clientProgress = new();
        private bool loadingActive = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
 
           //ceneManager.sceneLoaded += OnSceneLoaded;
        }

      /*  public override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= OnSceneLoaded;

        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            clientProgress.Clear();
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
                clientProgress[id] = 0f;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReportProgressServerRpc(float progress, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            clientProgress[clientId] = progress;

            if (AllClientsReady())
            {
                Debug.Log("✅ Tüm client'lar hazır, Main_Mp yükleniyor...");
                //Oyun sahnesi
                NetworkManager.Singleton.SceneManager.LoadScene("Main_Mp", LoadSceneMode.Single);
            }
        }

        private bool AllClientsReady()
        {
            foreach (var kvp in clientProgress)
                if (kvp.Value < 1f)
                    return false;
            return true;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            loadingActive = scene.name == "LoadingScene";

            if (loadingActive)
            {
                var ui = FindObjectOfType<NetworkLoadingUIControllers>();
                if (ui != null)
                {
                    ui.Init(this);
                }
            }
        }*/
    }
}