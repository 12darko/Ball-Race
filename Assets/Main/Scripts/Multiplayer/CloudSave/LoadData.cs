using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.Singleton;
using _Main.Scripts.Multiplayer.UnityCore.Auth;
using _Main.Scripts.Multiplayer.UnityCore.Auth.Platforms;
using EMA.Scripts.PatternClasses;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace Multiplayer.CloudSave
{
    public class LoadData : MonoSingleton<LoadData>
    {
        public async void Start()
        {
            // Initialization kodların aynen kalıyor...
            switch (PlatformTypesController.Instance.PlatformType)
            {
                case LoginPlatformType.Unity:
                    await UnityServices.InitializeAsync();
                    break;
                // ...
            }
        }

        public async Task<T> LoadPlayerDataValueForUnity<T>(string dataKey)
        {
            try
            {
                // Cloud'a soruyoruz: "Sende bu anahtar (örn: Coins) var mı?"
                var serverData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { dataKey });

                // Eğer varsa ve okunabiliyorsa
                if (serverData.TryGetValue(dataKey, out var value))
                {
                    return _Main.Scripts.Multiplayer.Mutlplayer.NetworkMisc.Misc.Deserialize<T>(value.Value.GetAsString());
                }
            }
            catch (Exception e)
            {
                // Hata çıkarsa (internet yoksa veya veri bozuksa) korkma, log at ve devam et
                Debug.LogWarning($"⚠️ {dataKey} yüklenemedi (Yeni oyuncu olabilir): {e.Message}");
            }

            // KRİTİK NOKTA: Veri yoksa veya hata varsa 'default' döndür.
            // int için 0, string için null, float için 0.0f döner.
            return default;
        }
    }
}