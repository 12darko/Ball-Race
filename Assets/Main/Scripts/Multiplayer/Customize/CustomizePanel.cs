using System.Collections.Generic;
using Multiplayer.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize
{
    public abstract class CustomizePanel : MonoBehaviour
    {
     [FormerlySerializedAs("CustomizeItems")] [SerializeField] protected List<CustomizeItem> customizeItems;
     [FormerlySerializedAs("CustomizeImageItem")] [SerializeField] protected CustomizeImageItem customizeImageItem;
     [SerializeField] protected Button equipBtn;
     [SerializeField] protected Button buyBtn;
 
     // --- YENİ EKLENEN: PARA YAZISI ---
     [Header("UI References")]
     [SerializeField] protected TMP_Text coinText;
      
     // --- YENİ EKLENEN KISIM: UI REFERANSLARI ---
        // Sahnedeki küçük kutuları ismine göre bulmamızı sağlayacak hafıza
        protected Dictionary<string, CustomizeImageItem> uiItemsDictionary = new Dictionary<string, CustomizeImageItem>();

        protected abstract void Show(CustomizeItem customizeItem);
// PARAYI GÜNCELLEME FONKSİYONU
        protected void UpdateCoinUI()
        {
            if (coinText != null)
            {
                // Parayı alıp yazıya döküyoruz
                coinText.text = PlayerData.Instance.PlayerCoins.ToString();
            }
        }
        protected void UpdateButtons(CustomizeItem item)
        {
            // ... (Eski UpdateButtons kodunun başı aynı) ...
            if (buyBtn != null) buyBtn.onClick.RemoveAllListeners();
            if (equipBtn != null) equipBtn.onClick.RemoveAllListeners();

            bool isOwned = !item.CustomizeItemIsLocked || PlayerData.Instance.IsItemOwned(item.name);

            if (isOwned)
            {
                // ... (Kuşanma kodları aynı) ...
                if (buyBtn != null) buyBtn.gameObject.SetActive(false);
                if (equipBtn != null)
                {
                    equipBtn.gameObject.SetActive(true);
                    var btnText = equipBtn.GetComponentInChildren<TMP_Text>();
                    if (btnText != null) btnText.text = "EQUIP";

                    equipBtn.onClick.AddListener(() =>
                    {
                        PlayerManager.Instance.SaveCosmeticsData();
                        Debug.Log($"{item.name} Kuşanıldı.");
                    });
                }
            }
            else
            {
                // ... (Satın alma kodları) ...
                if (equipBtn != null) equipBtn.gameObject.SetActive(false);
                if (buyBtn != null)
                {
                    buyBtn.gameObject.SetActive(true);
                    var priceText = buyBtn.GetComponentInChildren<TMP_Text>();
                    if (priceText != null) priceText.text = item.CustomizeItemPrice.ToString();

                    buyBtn.onClick.AddListener(() =>
                    {
                        if (PlayerData.Instance.TryPurchaseItem(item.name, item.CustomizeItemPrice))
                        {
                            Debug.Log("Satın Alma Başarılı!");

                            // --- KRİTİK DOKUNUŞ BURADA ---
                            // 1. Ekranı yenile (Butonlar değişsin)
                            UpdateButtons(item);

                            // 2. KÜÇÜK KUTUNUN KİLİDİNİ ANINDA KALDIR!
                            if (uiItemsDictionary.ContainsKey(item.name))
                            {
                                uiItemsDictionary[item.name].UnlockItemUI();
                            }
                            
                            // --- 3. PARA YAZISINI GÜNCELLE (AZALDIĞINI GÖRSÜN) ---
                            UpdateCoinUI();
                        }
                        else
                        {
                            Debug.LogWarning("Yetersiz Bakiye!");
                        }
                    });
                }
            }
        }
    }
}