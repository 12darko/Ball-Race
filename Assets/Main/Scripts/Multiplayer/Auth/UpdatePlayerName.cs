using System;
using System.Collections;
using System.Collections.Generic;
using Assets._Main.Scripts.UI;
using Multiplayer.Player;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayerName : MonoBehaviour
{

   [SerializeField] private GameObject updatePlayerPanel;
   [SerializeField] private TMP_InputField updatePlayerNameInputField;
   [SerializeField] private Button updatePlayerNameButton;

   private void Start()
   {
      updatePlayerNameButton.onClick.AddListener(ChangePlayerName);
        
      // Uygulama başladığında bu metodu çağırarak panelin görünürlüğünü bir kere kontrol edin.
      CheckAndShowPanel();
   }

   // Paneli göstermek için harici bir metot
   private void CheckAndShowPanel()
   {
      // Yalnızca isminiz boşsa paneli gösterir.
      if (string.IsNullOrEmpty(PlayerData.Instance.PlayerName))
      {
         ObjectController.SetActiveEffectController(updatePlayerPanel, null, .65f);
      }
   }

   private async void ChangePlayerName()
   {
      // Kullanıcının girdiği isim boşsa işlem yapma
      if (string.IsNullOrEmpty(updatePlayerNameInputField.text))
      {
         Debug.LogError("Lütfen geçerli bir isim giriniz.");
         return;
      }

      try
      {
         // 1. Önce Unity Authentication servisinde ismi güncelle
         await AuthenticationService.Instance.UpdatePlayerNameAsync(updatePlayerNameInputField.text);

         // 2. Ardından, PlayerData sınıfındaki ismi güncellemek için bir metot çağır.
         // Bu metot, PlayerData sınıfı içinde olması gerekir.
         PlayerData.Instance.SetPlayerName(updatePlayerNameInputField.text);

         // Paneli gizle
         updatePlayerPanel.SetActive(false);
         Debug.Log("İsminiz başarıyla güncellendi.");
      }
      catch (AuthenticationException ex)
      {
         Debug.LogException(ex);
         Debug.LogError("İsim güncelleme başarısız.");
      }
      catch (RequestFailedException ex)
      {
         Debug.LogException(ex);
         Debug.LogError("İstek başarısız.");
      }
   }
}
