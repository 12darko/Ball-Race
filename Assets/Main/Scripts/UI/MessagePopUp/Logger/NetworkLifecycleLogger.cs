using _Main.Scripts.UI.MessagePopUp.Logger;
using Assets._Main.Scripts.UI.MessagePopUp;
using Multiplayer;
 using Steamworks;
using Steamworks.Data;
 using UnityEngine;

public class NetworkLifecycleLogger : MonoBehaviour
{
    private bool _leaving;
    private static bool _isLeavingGlobal;

    private void OnEnable()
    {
     /*   if (NetworkManager.Singleton == null) return;

        var nm = NetworkManager.Singleton;
        nm.OnServerStarted += OnServerStarted;
        nm.OnServerStopped += OnServerStopped;
        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        try { SteamMatchmaking.OnLobbyMemberLeave += HandleOnLobbyMemberLeave; }
        catch { }*/
    }

    private void OnDisable()
    {
     /*   if (NetworkManager.Singleton == null) return;

        var nm = NetworkManager.Singleton;
        nm.OnServerStarted -= OnServerStarted;
        nm.OnServerStopped -= OnServerStopped;
        nm.OnClientConnectedCallback -= OnClientConnected;
        nm.OnClientDisconnectCallback -= OnClientDisconnected;

        try { SteamMatchmaking.OnLobbyMemberLeave -= HandleOnLobbyMemberLeave; }
        catch { }*/
    }

    // 🔹 Steam event — host lobiden çıkarsa client popup görecek
    /*   private void HandleOnLobbyMemberLeave(Lobby lobby, Friend member)
      {
         try
          {
             if (member.Id == lobby.Owner.Id && !NetworkManager.Singleton.IsHost)
              {
                  Debug.LogWarning("⚡ [Steam] Host lobby’den çıktı (Steam event).");
                  TriggerLeave("Sunucu kapatıldı, bağlantı sonlandırıldı.");
              }
          }
          catch { }*/
    }

  /*private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            var pm = MessageUI.Instance?.ProcessManager;
            pm?.SetStep(GameProcessStep.LobbyCreated);
            pm?.SetStep(GameProcessStep.RelayAllocating);
            pm?.SetStep(GameProcessStep.RelayConnected);
            pm?.SetStep(GameProcessStep.ConnectingToServer);
        }
    }

    private void OnServerStopped(bool wasHost)
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        if (nm.IsHost)
        {
            Debug.Log("[Lifecycle] Host oyunu kapattı (sessiz kapanış).");
            return;
        }

        Debug.Log("[Lifecycle] OnServerStopped (client) → Sunucu kapandı popup’ı gösteriliyor.");
        TriggerLeave("Sunucu kapatıldı, bağlantı sonlandırıldı.");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            MessageUI.Instance?.ProcessManager?.SetStep(GameProcessStep.ConnectedToServer);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        if (nm.IsClient && !nm.IsHost)
        {
            bool isHostDisconnect = clientId == 0;

            if (isHostDisconnect)
            {
                Debug.Log("[Lifecycle] Host bağlantısı kesildi (OnClientDisconnected).");
                TriggerLeave("Host oyunu kapattı, bağlantı kesildi.");
            }
            else if (clientId == nm.LocalClientId)
            {
                TriggerLeave("Sunucudan bağlantı koptu.");
            }
        }
    }

    private void TriggerLeave(string message)
    {
        if (_leaving || _isLeavingGlobal) return;
        _leaving = true;
        _isLeavingGlobal = true;

        bool isHost = NetworkManager.Singleton.IsHost;

        if (!isHost)
        {
            try
            {
                MessageUI.Instance?.ProcessManager?.ShowNormalError(message);
                Debug.Log($"[Lifecycle] Popup clientta gösterildi: {message}");
            }
            catch
            {
                Debug.LogWarning("⚠️ [Lifecycle] Popup gösterilemedi (MessageUI null olabilir)");
            }
        }
        else
        {
            Debug.Log("[Lifecycle] Host sessizce çıkıyor, popup gösterilmeyecek.");
        }

        StartCoroutine(DelayedLeave());
    }

    private System.Collections.IEnumerator DelayedLeave()
    {
        yield return null;
        var leave = FindObjectOfType<LobbyLeave>(false);
        if (leave != null)
        {
            Debug.Log("🏃‍♂️ DelayedLeave → LeaveLobby çağrıldı");
            leave.LeaveLobby();
        }
        _isLeavingGlobal = false; // yeniden giriş için izin ver
    }
}
*/