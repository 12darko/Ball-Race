using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize
{
    public class CustomizeImageItem : MonoBehaviour
    {
        [SerializeField] private Button itemButton;
        [SerializeField] private Image itemImageHolder;
        [SerializeField] private Image equippedImage;
        [SerializeField] private GameObject lockedObject;
        [SerializeField] private TMP_Text itemPrice;
        

        public Button ItemButton
        {
            get => itemButton;
            set => itemButton = value;
        }

        public Image ItemImageHolder
        {
            get => itemImageHolder;
            set => itemImageHolder = value;
        }

        public Image EquippedImage
        {
            get => equippedImage;
            set => equippedImage = value;
        }

        public GameObject LockedObject
        {
            get => lockedObject;
            set => lockedObject = value;
        }

        public TMP_Text ItemPrice
        {
            get => itemPrice;
            set => itemPrice = value;
        }
        
        public void UnlockItemUI()
        {
            if (lockedObject != null)
            {
                lockedObject.SetActive(false); // Kilit resmini kapat
                // İstersen fiyat yazısını da kapatabilirsin veya "Owned" yazdırabilirsin
                if(itemPrice != null) itemPrice.text = ""; 
            }
        }
    }
}