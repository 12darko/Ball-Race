using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Assets._Main.Scripts.UI.Settings
{
    public class SettingsGraphicsQualityDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown qualityDropdown;

        // String Table adı
        [SerializeField] private string tableName = "Language";

        // Quality key listesi (UI tablosunda tanımlı olmalı)
        [SerializeField] private List<string> qualityKeys = new List<string>();
        private List<int> dropdownToQualityMap = new List<int> { 1, 2, 3, 5 };
        private bool active = false;

        private IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;

            // Lokalize edilmiş metinleri al
            List<string> localizedOptions = new List<string>();
            for (int i = 0; i < qualityKeys.Count; i++)
            {
                string localized = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, qualityKeys[i]);
                localizedOptions.Add(localized);
            }

            // Dropdown’a ekle
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(localizedOptions);

            // Önceden kaydedilmiş kaliteyi al
            int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());

            // QualitySettings index’ini dropdown index’ine çevir
            int dropdownIndex = dropdownToQualityMap.IndexOf(savedQuality);
            if (dropdownIndex == -1) dropdownIndex = 0;

            qualityDropdown.value = dropdownIndex;
            qualityDropdown.RefreshShownValue();

            // QualitySettings’i uygula
            QualitySettings.SetQualityLevel(savedQuality);

            // Listener ekle
            qualityDropdown.onValueChanged.AddListener(ChangeQuality);

            active = true;
        }

        private void ChangeQuality(int index)
        {
            if (!active) return;

            // Dropdown index’ini QualitySettings index’ine çevir
            int qualityIndex = dropdownToQualityMap[index];

            QualitySettings.SetQualityLevel(qualityIndex);

            // Kaydet
            PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
            PlayerPrefs.Save();
        }
        void Update()
        {
            // Mevcut kalite seviyesi
            int currentQuality = QualitySettings.GetQualityLevel();
            string qualityName = QualitySettings.names[currentQuality];

            Debug.Log("Current Graphics Quality: " + qualityName);
        }
    }
}