using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Assets._Main.Scripts.UI.Settings
{
    public class SettingsLanguageDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown languageDropdown;

        private bool active = false;
        [SerializeField] private List<string> languageNames = new List<string>();
 

        private IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;

            // Dropdown’a sadece manuel isimleri ekle
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(languageNames);

            // Mevcut seçili dili bul
            int currentIndex = 0;
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[i])
                {
                    currentIndex = i;
                    break;
                }
            }

            languageDropdown.value = currentIndex;
            languageDropdown.RefreshShownValue();

            // Listener ekle
            languageDropdown.onValueChanged.AddListener(ChangeLanguage);

            active = true;
        }

        private void ChangeLanguage(int index)
        {
            if (!active) return;

            var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
            LocalizationSettings.SelectedLocale = selectedLocale;
        }
    }
}