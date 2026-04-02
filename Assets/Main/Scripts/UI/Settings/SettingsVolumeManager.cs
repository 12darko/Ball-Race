using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets._Main.Scripts.UI.Settings
{
    public class SettingsVolumeManager : MonoBehaviour
    {
        [Header("Components")] [SerializeField]
        private AudioMixer volumeMixer;

        [SerializeField] private Slider volumeSlider;

        [Header("String Variable")] [SerializeField]
        private string mixerExposerParameterString;


        private void Awake()
        {
            if (PlayerPrefs.HasKey(mixerExposerParameterString))
            {
                LoadVolume();
            }
        }

        private void Start()
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        private void SetVolume(float volume)
        {
            volumeMixer.SetFloat(mixerExposerParameterString, MathF.Log10(volume)* 20);
            PlayerPrefs.SetFloat(mixerExposerParameterString, volume);
          }

        private void LoadVolume()
        {
            var volume = PlayerPrefs.GetFloat(mixerExposerParameterString);
             volumeSlider.value = volume;
            SetVolume(volume);
        }
    }
}