using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.PopUp
{
    public class PopUpManager : MonoBehaviour
    {
        [SerializeField] private PopUpSectionMain popUpSectionMain;
        [SerializeField] private Transform popUpCanvas;
        [SerializeField] private PopUpType popUpType;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Button acceptButton;


        public PopUpSectionMain PopUpSectionMain => popUpSectionMain;

        private void Start()
        {
            acceptButton.onClick.AddListener((() => ClosePopUp(popUpSectionMain)));
        }

        public void OpenPopUp(string infoText, PopUpTitleInfo titleInfo, PopUpType type)
        {
            popUpType = type;
            titleText.text = titleInfo.ToString().ToUpper();
            
            
            switch (popUpType)
            {
                case PopUpType.Loader:
                    popUpSectionMain = GetComponentInChildren<PopUpLoaderSection>(true);
                    break;
                case PopUpType.Info:
                    popUpSectionMain = GetComponentInChildren<PopUpInfoSection>(true);
                    break;
                default:
                    popUpSectionMain = GetComponentInChildren<PopUpInfoSection>(true);
                    break;
            }

            if (popUpSectionMain == null)
            {
                return;
            }
            popUpSectionMain.SetSectionText(infoText);
            popUpSectionMain.SetSectionObjectActive(true);
            popUpCanvas.gameObject.SetActive(true);
            
        }

        public void ClosePopUp(PopUpSectionMain sectionMain)
        {
            sectionMain.SetSectionObjectActive(false);
            popUpCanvas.gameObject.SetActive(false);
        }
    }
}