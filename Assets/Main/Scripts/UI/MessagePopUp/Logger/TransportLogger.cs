using _Main.Scripts.UI.MessagePopUp.Logger;
 
using UnityEngine;

public class TransportLogger : MonoBehaviour
{
/*  private void Start()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.NetworkConfig.NetworkTransport.OnTransportEvent += HandleTransportEvent;
    }

    private void HandleTransportEvent(NetworkEvent eventType, ulong clientId, System.ArraySegment<byte> payload, float receiveTime)
    {
        switch (eventType)
        {
            case NetworkEvent.Disconnect:
                Debug.Log($"[Transport] Bağlantı kesildi: {clientId}");
                MessageUI.Instance.ProcessManager.SetStep(GameProcessStep.Disconnected);
                break;

            case NetworkEvent.TransportFailure:
                Debug.Log("[Transport] Transport hatası oluştu!");
                break;
        }
    }*/
}