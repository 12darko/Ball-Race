using System.Collections.Generic;
using _Main.Scripts.UI.MessagePopUp.Logger;
using TMPro;
using UnityEngine;

namespace Assets._Main.Scripts.UI.MessagePopUp
{
    public class ProcessMessageManager : MonoBehaviour
    {
         private GameProcessStep currentStep = GameProcessStep.None;
         public GameProcessStep CurrentStep => currentStep;
         [SerializeField] private TMP_Text messageText;
         private readonly Dictionary<GameProcessStep, string> stepMessages = new()
         {
             { GameProcessStep.CreatingLobby, "Lobi oluşturuluyor..." },
             { GameProcessStep.LobbyCreated, "Lobi oluşturuldu. Lobiye bağlanılıyor..." },
             { GameProcessStep.RelayAllocating, "Relay kuruluyor..." },
             { GameProcessStep.RelayConnected, "Relay bağlantısı tamamlandı." },
             { GameProcessStep.ConnectingToServer, "Sunucuya bağlanılıyor..." },
             { GameProcessStep.ConnectedToServer, "Bağlantı sağlandı!" },
             { GameProcessStep.Disconnecting, "Bağlantı kesiliyor..." },
             { GameProcessStep.Disconnected, "Bağlantı kesildi." },
         };
         
         
         public void SetStep(GameProcessStep step, string overrideMessage = null)
         {
             string message = overrideMessage ?? (stepMessages.ContainsKey(step) ? stepMessages[step] : null);
             if (string.IsNullOrEmpty(message)) return;

             if (MessagePopUpManager.Instance == null)
                 return;

             

            MessagePopUpManager.Instance.UpdateMessage(message);
             EnableCloseButton(step == GameProcessStep.Error);

             // 🔹 Başarılı adımlarda otomatik kapanma
             if (step == GameProcessStep.Complete || step == GameProcessStep.ConnectedToServer)
                 MessagePopUpManager.Instance.TriggerOpenMessage(message, 2.5f);
             
             Debug.Log($"[ProcessMessage] {message}");

         }
         
    

     

        private void EnableCloseButton(bool enable)
        {
            if (MessagePopUpManager.Instance == null)
                return;

            var btn = MessagePopUpManager.Instance.GetComponentInChildren<UnityEngine.UI.Button>(true);
            if (btn != null)
                btn.gameObject.SetActive(enable);
        }

        public void ShowFatalError(string reason, bool shouldExit = false)
        {
            string msg = $"<color=red>Hata:</color> {reason}\n";
            if (shouldExit)
                msg += "Oyun kapatılıyor...";

            MessagePopUpManager.Instance.TriggerOpenMessage(msg, -1f, shouldExit ? () => Application.Quit() : null);
            EnableCloseButton(true);
        }
        
        public void ShowNormalError(string reason, bool shouldExit = false)
        {
            string msg = $"<color=red>Hata:</color> {reason}\n";
            
            MessagePopUpManager.Instance.TriggerOpenMessage(msg, -1f);
            EnableCloseButton(true);
        }
    }
}