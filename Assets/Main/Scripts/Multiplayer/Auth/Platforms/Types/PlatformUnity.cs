using System;
using System.Threading.Tasks;
using Multiplayer.CloudSave;
using Multiplayer.Player;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Multiplayer.UnityCore.Auth.Platforms
{
    public static class PlatformUnity
    {
        public static string PlayerId;

        // Anonim Giriş
        public static async Task SignInWithUnityAnonymous(string userName, string password, LoadData loadData, SaveData saveData)
        { 
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Anonim Giriş Yapıldı. Veriler bekleniyor...");
                PlayerId = AuthenticationService.Instance.PlayerId;

                // --- DÜZELTME: "await" ekledik ---
                // Bu satır bitene kadar bir alt satıra (SceneManager'a) geçmez.
                await PlayerManager.Instance.PlayerLoadDataForUnity(loadData);

                Debug.Log("Veriler geldi, Menüye geçiliyor.");
                SceneManager.LoadScene(1); 
            }
            catch (Exception e)
            {
                Debug.LogError("Giriş Hatası: " + e.Message);
            }
        }
        // Kullanıcı Adı ile Giriş
        public static async void SignInWithUnity(string userName, string password, LoadData loadData, SaveData saveData)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(userName, password);
                
                // --- DÜZELTME: "await" ekledik ---
                await PlayerManager.Instance.PlayerLoadDataForUnity(loadData);
                
                SceneManager.LoadScene(1);
            }
            catch (Exception ex)
            {
                Debug.LogError("Giriş Hatası: " + ex.Message);
            }
        }
        
        // Kayıt Olma
        public static async Task SignUpWithUnityUsernamePasswordAsync(string username, string password, SaveData saveData)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log("Kayıt Başarılı + " + AuthenticationService.Instance.PlayerId);
                
                // Yeni kayıt sonrası varsayılan verileri kaydedebiliriz (Opsiyonel)
                // PlayerManager.Instance.SaveCosmeticsData();
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
                Debug.Log("Kayıt Başarısız");
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }

        // İsim Değiştirme
    /*    public static async void ChangePlayerNameToUnity(TMP_InputField updatePlayerNameInputField, GameObject updatePlayerPanel, CharacterManager characterManager)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(updatePlayerNameInputField.text);
                
                // İsim değişince PlayerData'yı güncelle
                PlayerData.Instance.SetName(AuthenticationService.Instance.PlayerName);
                
                // Diğer verileri tazelemek isteyebilirsin
                // PlayerData.Instance.SetCoins(await LoadData.Instance.LoadPlayerDataValueForUnity<float>("Coins"));
                
                // Varsayılan karakteri ayarla (Eğer gerekliyse)
                SetDefaultCharacterValues("Chibi Soldier Black", characterManager);
                
                updatePlayerPanel.gameObject.SetActive(false);
                Debug.Log("İsminiz Başarıyla Güncellendi...");
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }*/

        // Karakter Seçimi (Eski kodun, yeni sisteme uyarladım)
       /* public static void SelectCharacterForUnity(CharacterItem item)
        {
            PlayerData.Instance.SetCharacterPrefab(item.CharacterData.PlayersCharacter);
            PlayerData.Instance.SetSelectedCharacterName(item.CharacterData.CharacterName);
         
            // Tekli kayıt yerine toplu kayıt öneririm ama şimdilik bu kalsın:
            SaveData.Instance.SavePlayerDataForUnity("SelectedCharacter", PlayerData.Instance.SelectedCharacterName);
            Debug.Log($"Selected Character {item.CharacterData.CharacterName}");
        }
        */
        private static void SetDefaultCharacterValues(string characterName)
        {
            // Bu kısım senin karakter logic'ine bağlı, aynen bıraktım.
            PlayerData.Instance.SetSelectedCharacterName(characterName);
            SaveData.Instance.SavePlayerDataForUnity("SelectedCharacter", PlayerData.Instance.SelectedCharacterName);
        }
    }
}