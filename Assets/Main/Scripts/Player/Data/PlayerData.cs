using System;
using System.Collections.Generic;
 
using EMA.Scripts.PatternClasses;
using Fusion;
using Steamworks;
using UnityEngine;
using Unity.Services.Authentication;
using UnityEngine.Serialization;

#if FACEPUNCH_STEAMWORKS
using Steamworks;
#endif

namespace Multiplayer.Player
{
    public class PlayerData : MonoSingleton<PlayerData>
    {
        // --- MEVCUT DEĞİŞKENLERİN ---
        [FormerlySerializedAs("_playerName")] [SerializeField] private string playerName;
        [FormerlySerializedAs("_playerLevel")] [SerializeField] private int playerLevel;
        [FormerlySerializedAs("_playerCoins")] [SerializeField] private float playerCoins;
        [FormerlySerializedAs("_playerId")] [SerializeField] private string playerId;
        [FormerlySerializedAs("_lobbyId")] [SerializeField] private string lobbyId;
        [SerializeField] private int playerAuthority;
        [SerializeField] private string selectedMode;
        [SerializeField] private string selectedCharacterName;
        
   
        [SerializeField] private NetworkPrefabRef myCharacterPrefab;
        [SerializeField] private NetworkPrefabRef myBallPrefab;
        [SerializeField] private NetworkPrefabRef myHatPrefab;
        [SerializeField] private NetworkPrefabRef myFacePrefab;

        // --- YENİ EKLENEN KOZMETİK DEĞİŞKENLERİ ---
        [Header("Cosmetics")]
        [SerializeField] private int selectedHatIndex = 0;
        [SerializeField] private int selectedFaceIndex = 0;
        [SerializeField] private int selectedBallIndex = 0;

        // Satın alınan eşyaların isimlerini tutan liste (EKSİKTİ, EKLENDİ)
        [SerializeField] private List<string> ownedItemNames = new List<string>();

        #region Props

        public Steamworks.Data.Lobby CurrentLobby { get; set; }

        public string PlayerName => playerName;
        public string LobbyId { get => lobbyId; set => lobbyId = value; }
        public string PlayerId => playerId;
        public int PlayerLevel => playerLevel;
        
        public float PlayerCoins { get => playerCoins; set => playerCoins = value; }
        public int PlayerAuthority { get => playerAuthority; set => playerAuthority = value; }
        public string SelectedMode => selectedMode;
        public string SelectedCharacterName => selectedCharacterName;

        // Kozmetik Propertyleri
        public int SelectedHatIndex { get => selectedHatIndex; set => selectedHatIndex = value; }
        public int SelectedFaceIndex { get => selectedFaceIndex; set => selectedFaceIndex = value; }
        public int SelectedBallIndex { get => selectedBallIndex; set => selectedBallIndex = value; }
        
        // Satın alınanlar listesine erişim
        public List<string> OwnedItemNames { get => ownedItemNames; set => ownedItemNames = value; }

 

        public event Action<string> OnModeChanged;

        #endregion

        // --- FONKSİYONLAR ---

        public void SetMode(string modeName)
        {
            selectedMode = modeName;
            OnModeChanged?.Invoke(modeName);
        }

        public void SetInitialData(string id)
        {
            playerId = id;
            if (!SteamClient.IsValid)
            {
                playerName = "Oyuncu_" + playerId.Substring(0, 4);
            }
        }

        public void SetPlayerName(string name) => playerName = name;
        public void SetLobbyId(string id) => lobbyId = id;
        public void SetLevel(int level) => playerLevel = level;
        public void SetCoins(float coins) => playerCoins = coins;

        public void SetLobby(Steamworks.Data.Lobby lobby)
        {
            CurrentLobby = lobby;
            Debug.Log("Lobby set: " + lobby.Id.Value);
        }

        public void ResetLobbyData()
        {
            Debug.Log("🧹 PlayerData reset: Lobby bilgileri sıfırlanıyor...");
            CurrentLobby = default;
            lobbyId = string.Empty;
            selectedMode = string.Empty;
            Debug.Log("✅ PlayerData: Lobi bilgileri başarıyla temizlendi.");
        }

        // --- EŞYA KONTROL VE SATIN ALMA ---

        // Bu eşya bizde var mı?
        public bool IsItemOwned(string itemName)
        {
            return ownedItemNames.Contains(itemName);
        }

        public bool TryPurchaseItem(string itemName, float cost)
        {
            if (playerCoins >= cost)
            {
                playerCoins -= cost; // Parayı düş
                ownedItemNames.Add(itemName); // Listeye ekle
                
                // İşlem başarılı, hemen PlayerManager üzerinden Cloud'a kaydet!
                PlayerManager.Instance.SaveCosmeticsData();
                
                return true; // Başarılı
            }
            return false; // Yetersiz Bakiye
        }

        public void SetName(string playerName) => this.playerName = playerName;
        public void SetSelectedCharacterName(string selectedCharacterName) => this.selectedCharacterName = selectedCharacterName;
        public bool IsInLobby => CurrentLobby.Id.IsValid;
        
        public NetworkPrefabRef SetCharacterPrefab(NetworkPrefabRef selectedCharacter) => myCharacterPrefab = selectedCharacter;
    }
}