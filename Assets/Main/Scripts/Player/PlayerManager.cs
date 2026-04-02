using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.UnityCore.Auth;
using _Main.Scripts.Multiplayer.UnityCore.Auth.Platforms;
using EMA.Scripts.PatternClasses;
using Multiplayer.CloudSave;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Multiplayer.Player
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        private async void Start()
        {
            // Platform başlatma (Login tipine göre)
            if (PlatformTypesController.Instance != null)
            {
                switch (PlatformTypesController.Instance.PlatformType)
                {
                    case LoginPlatformType.Unity:
                        await UnityServices.InitializeAsync();
                        break;
                    case LoginPlatformType.PlayFab:
                        break;
                    case LoginPlatformType.PlayerPrefs:
                        break;
                    default:
                        // InitializeAsync varsayılan olarak çağrılabilir
                         await UnityServices.InitializeAsync();
                        break;
                }
            }
            else
            {
                // Controller yoksa güvenli mod
                await UnityServices.InitializeAsync();
            }
        }

        // --- VERİ YÜKLEME (Login Olunca Çağrılır) ---
      public async Task PlayerLoadDataForUnity(LoadData loadData)
        {        
            Debug.Log("📥 PlayerManager: Veriler Yükleniyor... (Veri yoksa varsayılanlar ayarlanacak)");

            // 1. İsim
            PlayerData.Instance.SetName(AuthenticationService.Instance.PlayerName);
            
            // 2. PARA KONTROLÜ (Başlangıç Hediyesi)
            float coins = await loadData.LoadPlayerDataValueForUnity<float>("Coins");
            
            // EĞER SIFIR GELDİYSE (Yeni Oyuncu) -> 5000 VER
            if (coins <= 0) 
            {
                Debug.Log("🆕 Yeni Oyuncu Tespit Edildi! Başlangıç parası veriliyor.");
                coins = 5000; 
                
                // İstersen burada "İlk kayıt hediyesi" diye Cloud'a hemen yazabilirsin ama 
                // harcama yapınca kaydetmek daha performanslıdır. Şimdilik hafızada tutuyoruz.
            }
            PlayerData.Instance.SetCoins(coins);

            // 3. LEVEL KONTROLÜ
            int level = await loadData.LoadPlayerDataValueForUnity<int>("Level");
            // Level 0 olamaz, en az 1 olsun
            PlayerData.Instance.SetLevel(level <= 0 ? 1 : level);

            // 4. KARAKTER İSMİ
            string charName = await loadData.LoadPlayerDataValueForUnity<string>("SelectedCharacter");
            if (string.IsNullOrEmpty(charName))
            {
                // Veri yoksa varsayılan karakteri seç
                charName = "Chibi Soldier Black"; // Buraya varsayılan karakter adını yaz
            }
            PlayerData.Instance.SetSelectedCharacterName(charName);

            // 5. SATIN ALINANLAR (Envanter)
            var ownedItems = await loadData.LoadPlayerDataValueForUnity<List<string>>("OwnedItems");
            // Eğer null gelirse (hiçbir şey almamışsa) BOŞ LİSTE oluştur ki hata vermesin
            PlayerData.Instance.OwnedItemNames = ownedItems ?? new List<string>();

            // 6. KOZMETİK SEÇİMLERİ (Varsayılan 0)
            // LoadData veri bulamazsa zaten 0 döndürür, o yüzden ekstra if gerekmez.
            // 0 demek listenin ilk elemanı (varsayılan şapka) demektir.
            PlayerData.Instance.SelectedHatIndex = await loadData.LoadPlayerDataValueForUnity<int>("SelectedHatIndex");
            PlayerData.Instance.SelectedFaceIndex = await loadData.LoadPlayerDataValueForUnity<int>("SelectedFaceIndex");
            PlayerData.Instance.SelectedBallIndex = await loadData.LoadPlayerDataValueForUnity<int>("SelectedBallIndex");
            
            Debug.Log("✅ Veri Yükleme Tamamlandı. Sahneye geçiliyor.");
        }

        // --- KOZMETİK & PARA KAYDETME (Toplu İşlem) ---
        public void SaveCosmeticsData()
        {
            // Dictionary oluşturup SaveData'ya "Bunları topluca kaydet" diyoruz.
            // Böylece hem para düşüyor hem liste güncelleniyor, senkronize oluyor.
            
            var dataToSave = new Dictionary<string, object>
            {
                { "Coins", PlayerData.Instance.PlayerCoins }, // Güncel Para
                { "OwnedItems", PlayerData.Instance.OwnedItemNames }, // Satın Alınanlar Listesi
                { "SelectedHatIndex", PlayerData.Instance.SelectedHatIndex }, // Şapka Seçimi
                { "SelectedFaceIndex", PlayerData.Instance.SelectedFaceIndex }, // Yüz Seçimi
                { "SelectedBallIndex", PlayerData.Instance.SelectedBallIndex }, // Top Seçimi
                // Level ve Karakteri de buraya ekleyebilirsin, garanti olsun
                { "Level", PlayerData.Instance.PlayerLevel },
                { "SelectedCharacter", PlayerData.Instance.SelectedCharacterName }
            };

            Debug.Log("💾 PlayerManager: Veriler Cloud'a kaydediliyor...");
            SaveData.Instance.SaveBatchDataForUnity(dataToSave);
        }
        
        // Tekli kayıt (Eski metodları desteklemek için)
        public void PlayerSaveDataForUnity(SaveData saveData)
        {
            if (string.IsNullOrEmpty(PlayerData.Instance.PlayerName)) return;
            // Burası yerine SaveCosmeticsData kullanılması önerilir ama kalsın
            saveData.SavePlayerDataForUnity("Coins", PlayerData.Instance.PlayerCoins.ToString());
            saveData.SavePlayerDataForUnity("Level", PlayerData.Instance.PlayerLevel.ToString());
        }
    }
}