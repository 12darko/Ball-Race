using TMPro;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.PopUp
{
    public abstract class PopUpSectionMain : MonoBehaviour
    {
        [SerializeField] protected GameObject sectionObject;
        [SerializeField] protected TMP_Text sectionText;

      

        public abstract void SetSectionObjectActive(bool sectionObjectActive);
        public abstract void SetSectionText(string sectionInfoText);
    }
}