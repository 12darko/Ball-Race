// ... usingler ...

using Main.Scripts.Player.Database;
using Multiplayer.Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize.Hats
{
    public class HatsPanel : CustomizePanel
    {
        [SerializeField] private CosmeticDatabase cosmeticDB;
        [SerializeField] private Transform hatsContent;
        // Parentlara gerek yok

        private void Start()
        {
            customizeItems = cosmeticDB.allHats;
            UpdateCoinUI();
            uiItemsDictionary.Clear();

            for (int i = 0; i < customizeItems.Count; i++)
            {
                int index = i;
                var hatItem = customizeItems[i];
                var itemUI = Instantiate(customizeImageItem, hatsContent);
                itemUI.ItemImageHolder.sprite = hatItem.CustomizeItemImage;

                if (!uiItemsDictionary.ContainsKey(hatItem.name))
                    uiItemsDictionary.Add(hatItem.name, itemUI);

                itemUI.ItemButton.onClick.AddListener(() =>
                {
                    // --- MÜDÜRE SÖYLE ---
                    CustomizePreview.Instance.UpdateHatVisual(hatItem);
                    
                    PlayerData.Instance.SelectedHatIndex = index;
                    UpdateButtons(hatItem);
                });

                // ... (Kilit kontrol kodların aynen kalıyor) ...
                bool actuallyLocked = hatItem.CustomizeItemIsLocked && !PlayerData.Instance.IsItemOwned(hatItem.name);
                if (actuallyLocked) { /*...*/ } else { /*...*/ }
            }
        }
        
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