// ... usingler ...

using Main.Scripts.Player.Database;
using Multiplayer.Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize.Ball
{
    public class BallPanel : CustomizePanel
    {
        [SerializeField] private CosmeticDatabase cosmeticDB;
        [SerializeField] private Transform ballContent;
        // Parent değişkenlerine gerek kalmadı, Manager hallediyor
        
        // GameObject referansına da gerek kalmadı
        public GameObject CreatedItemBall => CustomizePreview.Instance.CurrentBallVisual; 

        private void Start()
        {
            customizeItems = cosmeticDB.allBalls;
            UpdateCoinUI();
            uiItemsDictionary.Clear();

            for (int i = 0; i < customizeItems.Count; i++)
            {
                int index = i;
                var ballItem = customizeItems[i];
                var itemUI = Instantiate(customizeImageItem, ballContent);
                itemUI.ItemImageHolder.sprite = ballItem.CustomizeItemImage;

                if (!uiItemsDictionary.ContainsKey(ballItem.name))
                    uiItemsDictionary.Add(ballItem.name, itemUI);

                itemUI.ItemButton.onClick.AddListener(() =>
                {
                    // --- ARTIK KENDİSİ YARATMIYOR, MÜDÜRE SÖYLÜYOR ---
                    CustomizePreview.Instance.UpdateBallVisual(ballItem);
                    
                    PlayerData.Instance.SelectedBallIndex = index;
                    UpdateButtons(ballItem);
                });
                
                // ... (Kilit/Fiyat kontrol kodların aynen kalıyor) ...
                bool actuallyLocked = ballItem.CustomizeItemIsLocked && !PlayerData.Instance.IsItemOwned(ballItem.name);
                if (actuallyLocked) { /*...*/ } else { /*...*/ }
            }
            // Start'ta preview yüklemeye gerek yok, Manager zaten Start'ta hepsini yüklüyor!
        }

        // Show metodunu override edip içini boşaltabilirsin veya buton listener'da direkt manager çağırdık zaten.
        protected override void Show(CustomizeItem customizeItem)
        {
             // Buradaki kodları sildik çünkü Manager yapıyor.
             // Sadece butonları güncellemek için kullanabilirsin:
             
             if (customizeItem.CustomizeItemIsLocked)
             {
                 equipBtn.gameObject.SetActive(false);
                 buyBtn.gameObject.SetActive(true);
             }
             else
             {
                 buyBtn.gameObject.SetActive(false);
                 equipBtn.gameObject.SetActive(true);
             }
             UpdateButtons(customizeItem);
        }
    }
}