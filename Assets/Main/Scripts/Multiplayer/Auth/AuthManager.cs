using System;
using Multiplayer.Auth.Anonymous;
using Other;
 using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace Multiplayer.Auth
{
    public class AuthManager : MonoBehaviour
    {
        [Header("Authentication Type")] [SerializeField]
        private AuthType authType;

        [Header("Assigned Components")] [SerializeField]
        private AuthComponents anonymousComponents;

        [SerializeField] private Login anonymousLogin;
        [Header("Steam")] [SerializeField] private AuthComponents steamComponents; // Yeni
        [SerializeField] private Login steamLogin; // Yeni
        [Header("User")] [SerializeField] private AuthComponents userComponents; // Yeni
        [SerializeField] private Login userLogin; // Yeni
        [Header("Managers")]
        [SerializeField] private SteamManager steamManager; // ⚠️ SteamManager'ı buraya ataman lazım
        
        // 🔊 Yeni: Ses için referanslar
        [Header("Audio")] 
        [SerializeField] private AudioMixer mainMixer;
        
        // Ses parametrelerinin isimleri (AudioMixer'deki Exposed parametrelerle aynı olmalı)
        [SerializeField] private string[] exposedParameters = { "Music", "Game", "SFX" };
        private const float DefaultVolume = 0.5f;
    
        private void Start()
        {
            // Ses ayarlarını başlat
            InitializeDefaultAudioSettings();
            SetCurrentAuthType();
        }
        private void InitializeDefaultAudioSettings()
        {
            foreach (var param in exposedParameters)
            {
                if (!PlayerPrefs.HasKey(param))
                {
                    PlayerPrefs.SetFloat(param, DefaultVolume);
                    PlayerPrefs.Save();
                }

                float volume = PlayerPrefs.GetFloat(param, DefaultVolume);
                float db = (volume > 0.0001f) ? Mathf.Log10(volume) * 20f : -80f;
                mainMixer.SetFloat(param, db);
            }
        }
        private void SetCurrentAuthType()
        {
            AuthComponents currentAuthComponents = null;
            Login currentLoginType = null;
            // Eğer Steam seçilmişse, Steam'i başlat
            if (authType is AuthType.Steam) // anon şimdilik
            {
                if (steamManager != null)
                {
                    steamManager.InitializeSteam();
                }
                else
                {
                    Debug.LogError("SteamManager atanmamış!");
                    return;
                }
            }
            switch (authType)
            {
                case AuthType.Anonymous:
                    currentAuthComponents = anonymousComponents;
                    currentLoginType = anonymousLogin;
                  //  TransportPlatformController.Instance.PlatformTypes = TransportPlatformTypes.UNITY_RELAY;
                    break;
                case AuthType.Steam: // Yeni
                    currentAuthComponents = steamComponents;
                    currentLoginType = steamLogin;
               //     TransportPlatformController.Instance.PlatformTypes = TransportPlatformTypes.STEAM;
                    break;
                // ... Diğer tipler buraya eklenebilir
                case AuthType.User: // Yeni
                    currentAuthComponents = userComponents;
                    currentLoginType = userLogin;
                    //TransportPlatformController.Instance.PlatformTypes = TransportPlatformTypes.UNITY_RELAY;
                    break;
                case AuthType.Google:
                case AuthType.Facebook:
                case AuthType.GooglePlay:
                case AuthType.AnonymousUnity_Steam:
                    currentAuthComponents = anonymousComponents;
                    currentLoginType = anonymousLogin;
                  //  TransportPlatformController.Instance.PlatformTypes = TransportPlatformTypes.STEAM;
                    break;
                default:
                    Debug.LogError($"Bilinmeyen Kimlik Doğrulama Tipi: {authType}");
                    return;
            }

            if (currentAuthComponents != null && currentLoginType != null)
            {
                currentAuthComponents.TogglePanel(true);
                currentAuthComponents.OnLoginButtonClicked.AddListener(async () =>
                {
                    currentAuthComponents.LoginBtn.enabled = false;
                    await currentLoginType.SignInAsync();
                });
                   
            }
            else
            {
                Debug.LogError("Kimlik doğrulama bileşenleri doğru atanmamış!");
            }
        }
    }
}