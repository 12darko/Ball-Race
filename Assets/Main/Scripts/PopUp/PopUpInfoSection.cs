using TMPro;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.PopUp
{
    public class PopUpInfoSection : PopUpSectionMain
    {
        public override void SetSectionObjectActive(bool sectionObjectActive)
        {
            sectionObject.SetActive(sectionObjectActive);
        }

        public override void SetSectionText(string sectionInfoText)
        {
            sectionText.text = sectionInfoText;
        }
    }
}