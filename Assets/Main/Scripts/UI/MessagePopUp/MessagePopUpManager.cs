using System;
using EMA.Scripts.PatternClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Main.Scripts.UI.MessagePopUp
{
    public class MessagePopUpManager : MonoSingleton<MessagePopUpManager>
    {
        [Header("Components")]
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private GameObject popUpGameObject;
        [SerializeField] private Button closePopUpButton;

        // Eventler (UI ile etkileşimler için)
        public event EventHandler OnOpenMessage;
        public event EventHandler OnCloseMessage;
        private Action onCloseAction;

        private bool isOpen = false;
        private string currentMessage;
        private float autoCloseDelay = -1f;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            closePopUpButton.onClick.AddListener(ClosePopUp);
        }

        private void Update()
        {
            if (isOpen && autoCloseDelay > 0)
            {
                autoCloseDelay -= Time.deltaTime;
                if (autoCloseDelay <= 0)
                {
                    ClosePopUp();
                }
            }
        }

        /// <summary>
        /// Mesaj popup'ını açar (varsa günceller)
        /// </summary>
        public void TriggerOpenMessage(string message, float autoCloseTime = -1f, Action onClose = null)
        {
            if (!isOpen)
            {
                ObjectController.SetActiveEffectControllerMessageBox(popUpGameObject, null, .65f);
                isOpen = true;
                OnOpenMessage?.Invoke(this, EventArgs.Empty);
                currentMessage = ""; // 🔹 yeni açıldığında sadece burada sıfırla
            }

            // 🔹 Eğer hali hazırda bir mesaj varsa altına ekle
            currentMessage = message;
 
            messageText.text = currentMessage;
            autoCloseDelay = autoCloseTime;
            onCloseAction = onClose; // 🔹 callback sakla
        }
        /// <summary>
        /// Popup açıkken metni canlı olarak günceller
        /// </summary>
        public void UpdateMessage(string newMessage)
        {
            if (!isOpen) 
            {
                TriggerOpenMessage(newMessage);
                return;
            }
 
            currentMessage = newMessage;
            messageText.text = "";
            Debug.Log(currentMessage +  "Şimdiki ");
            messageText.text = currentMessage;
        }
       
        
     
        /// <summary>
        /// Popup'ı kapatır
        /// </summary>
        public void ClosePopUp()
        {
            if (!isOpen) return;

            ObjectController.SetActiveEffectControllerMessageBox(null, popUpGameObject, .65f);
            isOpen = false;
            autoCloseDelay = -1f;

            onCloseAction?.Invoke(); // 🔹 özel davranış çalıştır
            onCloseAction = null;

            OnCloseMessage?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Popup açık mı?
        /// </summary>
        public bool IsOpen => isOpen;
    }
}
