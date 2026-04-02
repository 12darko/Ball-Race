using System;
using System.Collections.Generic;
using _Main.Scripts.Multiplayer.Singleton;
using _Main.Scripts.Multiplayer.UnityCore.Auth;
using _Main.Scripts.Multiplayer.UnityCore.Auth.Platforms;
using EMA.Scripts.PatternClasses;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace Multiplayer.CloudSave
{
    public class SaveData : MonoSingleton<SaveData>
    {
        public async void Start()
        {
            // Initialization kodların aynen kalıyor...
            switch (PlatformTypesController.Instance.PlatformType)
            {
                case LoginPlatformType.Unity:
                    await UnityServices.InitializeAsync();
                    break;
                // Diğer caseler...
            }
        }

        // --- MEVCUT METODUN (Tekli String Kayıt) ---
        public async void SavePlayerDataForUnity(string dataKey, string dataValue)
        {
            var data = new Dictionary<string, object> { { dataKey, dataValue } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log($"✅ {dataKey} kaydedildi.");
        }
        
        // --- [YENİ] TOPLU KAYIT METODU (Performans için şart) ---
        // Dictionary alır, hepsini tek pakette sunucuya atar.
        public async void SaveBatchDataForUnity(Dictionary<string, object> dataDict)
        {
            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(dataDict);
                Debug.Log("✅ Toplu veri kaydı başarılı.");
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Toplu Kayıt Hatası: " + e.Message);
            }
        }
    }
}