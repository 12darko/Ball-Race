using Fusion;
using Multiplayer.Player;
using Player.Cosmetic;
using Player.Cosmetic.Faces;
using UnityEngine;

namespace Player
{
    public class NetworkPlayerInGameData : NetworkBehaviour
    {
        [Networked] public NetworkString<_32> PlayerName { get; set; }
        [Networked] public int PlayerId { get; set; }
        [Networked] public int PlayerFallingCount { get; set; }
        [Networked] public int PlayerWinScore { get; set; }
        
        // --- YENİ EKLENENLER: KOZMETİK INDEXLERİ ---
        [Networked] public int NetworkedHatIndex { get; set; }
        [Networked] public int NetworkedFaceIndex { get; set; }

        // Managerlara erişim için referanslar (Inspector'dan sürükle veya kodla bul)
        [SerializeField] private NetworkPlayersHatManager hatManager;
        [SerializeField] private NetworkPlayersFaceManager faceManager;

        
        private ChangeDetector _changeDetector;
        
        private void Start()
        {
          
        }
        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            // Eğer bu karakter benim kontrolümdeyse (Local Player)
            if (Object.HasInputAuthority)
            {
                SetLocalObject();
            }
            
            
            // ✅ Server'da spawn olunca mevcut değerlerle direkt tetikle
            // faceIndex 0 olsa bile çalışır
            if (Runner.IsServer)
            {
                OnCosmeticUpdated();
            }
        }

        
        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(PlayerName):
                        OnChangedNickName();
                        break;
                    // Kozmetik verisi sunucuya ulaştığında tetiklenir
                    case nameof(NetworkedHatIndex):
                    case nameof(NetworkedFaceIndex):
                        OnCosmeticUpdated();
                        break;
                }
            }
        }
        
        private void SetPlayerName(NetworkString<_32> val)
        {
            PlayerName = val;
        }
        
        private void SetLocalObject()
        {
            PlayerId = Object.InputAuthority.PlayerId;

            // İsim Gönderme
            if (!string.IsNullOrEmpty(PlayerData.Instance.PlayerName))
            {
                RPC_SetNickName(PlayerData.Instance.PlayerName);
            }

            // --- KOZMETİK GÖNDERME ---
            // PlayerData'dan seçili indexleri alıp Server'a gönderiyoruz
            RPC_SetCosmetics(PlayerData.Instance.SelectedHatIndex, PlayerData.Instance.SelectedFaceIndex);
        }

        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetNickName(NetworkString<_32> nickName)
        {
            PlayerName = nickName;
        }
// Yeni RPC: Client seçimini Server'a yazar
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetCosmetics(int hatIndex, int faceIndex)
        {
            NetworkedHatIndex = hatIndex;
            NetworkedFaceIndex = faceIndex;
            
            // RPC sunucuda çalışır, değişkenler değişince Render'daki ChangeDetector yakalar
            // Ama garanti olsun diye direkt burada da çağırabiliriz:
            OnCosmeticUpdated();
        }
        private void OnChangedNickName()
        {
            SetPlayerName(PlayerName.ToString());
        }
        
        // Kozmetik değişince Managerları tetikleme (SADECE SERVERDA ÇALIŞSA YETERLİ, ÇÜNKÜ SPAWN SERVERDA)
        private void OnCosmeticUpdated()
        {
            if (Runner.IsServer)
            {
                // Managerlara "Şu indexi spawn et" diyoruz
                if(hatManager != null) hatManager.SpawnHat(NetworkedHatIndex);
                if(faceManager != null) faceManager.SpawnFace(NetworkedFaceIndex);
            }
        }
    }
}