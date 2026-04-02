using System.Globalization;
using Main.Scripts.Player.Database;
using Multiplayer.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize.Faces
{
    public class FacesPanel : CustomizePanel
    {
        // 1. Database Referansı (Listeyi buradan çekeceğiz)
        [SerializeField] private CosmeticDatabase cosmeticDB; 

        // Sadece UI'daki butonların dizileceği yer (Scroll View Content)
        [SerializeField] private Transform facesContent;

        // NOT: facesParent, ballPanel veya createdItemFaces değişkenlerini SİLDİK.
        // Çünkü artık görselleri MarketPreviewManager yönetiyor.

        private void Start()
        {
            // Listeyi Database'den çek
            customizeItems = cosmeticDB.allFaces; 
            
            UpdateCoinUI();
            uiItemsDictionary.Clear(); 

            for (int i = 0; i < customizeItems.Count; i++)
            {
                int index = i;
                var faceItem = customizeItems[i];

                // UI Butonunu oluştur
                var itemUI = Instantiate(customizeImageItem, facesContent);
                itemUI.ItemImageHolder.sprite = faceItem.CustomizeItemImage;
                
                if (!uiItemsDictionary.ContainsKey(faceItem.name))
                    uiItemsDictionary.Add(faceItem.name, itemUI);

                // --- BUTONA TIKLANINCA ---
                itemUI.ItemButton.onClick.AddListener(() =>
                {
                    // 1. Müdüre Söyle: "Bu yüzü tak"
                    CustomizePreview.Instance.UpdateFaceVisual(faceItem);
                    
                    // 2. Hafızaya Kaydet
                    PlayerData.Instance.SelectedFaceIndex = index;

                    // 3. UI Butonlarını Güncelle (Satın Al / Kuşan)
                    Show(faceItem);
                });

                // Kilit ve Fiyat Kontrolü
                bool actuallyLocked = faceItem.CustomizeItemIsLocked && !PlayerData.Instance.IsItemOwned(faceItem.name);

                if (actuallyLocked)
                {
                    itemUI.LockedObject.gameObject.SetActive(true);
                    itemUI.ItemPrice.text = faceItem.CustomizeItemPrice.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    itemUI.LockedObject.gameObject.SetActive(false);
                }
            }
        }

        // Bu fonksiyon artık sadece butonları (Buy/Equip) ayarlıyor.
        // Görsel yaratma işi Manager'da olduğu için burası sadeleşti.
        protected override void Show(CustomizeItem customizeItem)
        {
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