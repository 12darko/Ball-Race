using UnityEngine;
using Assets._Main.Scripts.UI.MessagePopUp;

namespace Assets._Main.Scripts.Utils
{
    public static class GameFlowLogger
    {
        
        
        public static void Info(string msg)
        {
            Debug.Log("[GameFlow] " + msg);
            TryShowOnUI(msg);
        }

        public static void Warn(string msg)
        {
            Debug.LogWarning("[GameFlow] ⚠ " + msg);
            TryShowOnUI("<color=yellow>" + msg + "</color>");
        }

        public static void Error(string msg)
        {
            Debug.LogError("[GameFlow] ❌ " + msg);
            TryShowOnUI("<color=red>" + msg + "</color>");
        }

        private static void TryShowOnUI(string msg)
        {
            // 🔹 Kullanıcıya gösterilmemesi gereken sistem mesajlarını filtrele
            if (msg.Contains("[NetworkLifecycle]") || msg.Contains("Client ID") || msg.Contains("Host"))
                return;

            if (MessagePopUpManager.Instance != null)
            {
                MessagePopUpManager.Instance.UpdateMessage(msg);
            }
        }
    }
}