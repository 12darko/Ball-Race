using System.Collections.Generic;
using Main.Scripts.Player.Database;
using Multiplayer.Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize
{
    public class CustomizePreview : MonoBehaviour
    {
        public static CustomizePreview Instance;

        [Header("Database & Parents")] [SerializeField]
        private CosmeticDatabase cosmeticDB;

        [SerializeField] private Transform ballParent;
        [SerializeField] private Transform hatsParent;
        [SerializeField] private Transform facesParent;

        // Şu an sahnede duran görseller
        public GameObject CurrentBallVisual { get; private set; }
        private GameObject _currentHatVisual;
        private GameObject _currentFaceVisual;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            // Market açılır açılmaz, hiçbir sekmeye basılmadan her şeyi yükle!
            LoadFullCharacter();
        }

        public void LoadFullCharacter()
        {
            // 1. Kayıtlı Topu Yükle
            int savedBall = PlayerData.Instance.SelectedBallIndex;
            if (IsValid(savedBall, cosmeticDB.allBalls))
                UpdateBallVisual(cosmeticDB.allBalls[savedBall]);

            // 2. Kayıtlı Şapkayı Yükle
            int savedHat = PlayerData.Instance.SelectedHatIndex;
            if (IsValid(savedHat, cosmeticDB.allHats))
                UpdateHatVisual(cosmeticDB.allHats[savedHat]);

            // 3. Kayıtlı Yüzü Yükle
            int savedFace = PlayerData.Instance.SelectedFaceIndex;
            if (IsValid(savedFace, cosmeticDB.allFaces))
                UpdateFaceVisual(cosmeticDB.allFaces[savedFace]);
        }

        // --- GÖRSEL GÜNCELLEME FONKSİYONLARI ---

        public void UpdateBallVisual(CustomizeItem item)
        {
            if (CurrentBallVisual != null) Destroy(CurrentBallVisual);

            CurrentBallVisual = Instantiate(item.CustomizeItemModel, ballParent);
            CurrentBallVisual.transform.localPosition = Vector3.zero;
            CurrentBallVisual.transform.localRotation = Quaternion.identity;
            // Topun scale ayarı varsa buraya eklersin
        }

        public void UpdateHatVisual(CustomizeItem item)
        {
            if (CurrentBallVisual == null) return; // Top yoksa şapka takma
            if (_currentHatVisual != null) Destroy(_currentHatVisual);

            _currentHatVisual = Instantiate(item.CustomizeItemModel, hatsParent);
            _currentHatVisual.transform.localPosition = item.CustomizeItemSpawnPosition;
            // Şapka için özel rotasyonun vardı (-89), onu koruyoruz
            _currentHatVisual.transform.localRotation = Quaternion.Euler(new Vector3(-89f, 0, 0));
            _currentHatVisual.transform.localScale = Vector3.one * item.CustomizeItemScale;
        }

        public void UpdateFaceVisual(CustomizeItem item)
        {
            if (CurrentBallVisual == null) return;
            if (_currentFaceVisual != null) Destroy(_currentFaceVisual);

            _currentFaceVisual = Instantiate(item.CustomizeItemModel, facesParent);
            _currentFaceVisual.transform.localPosition = item.CustomizeItemSpawnPosition;
            // Yüz için ScriptableObject rotasyonunu kullanıyoruz
            _currentFaceVisual.transform.localRotation = Quaternion.Euler(item.CustomizeItemSpawnRotation);
            _currentFaceVisual.transform.localScale = Vector3.one * item.CustomizeItemScale;
        }

        // Yardımcı: Index geçerli mi?
        private bool IsValid(int index, List<CustomizeItem> list)
        {
            return index >= 0 && index < list.Count;
        }
    }
}

 