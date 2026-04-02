using System;
using _Main.Scripts.Multiplayer.Runners;
using _Main.Scripts.Multiplayer.Singleton;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.Multiplayer.Lobby
{
    public class LobbyPlayerMapSelector : MonoBehaviour
    {
        [SerializeField] private TMP_Text selectedMapNameText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private string[] mapList;
        [SerializeField] private Sprite[] mapImageList;
        [SerializeField] private Image selectedMaSprite;
        private int _selectedMapCountIndex;
        private string _selectedMapName;

        private void Start()
        {
            nextButton.onClick.AddListener(Next);
            previousButton.onClick.AddListener(Previous);
            // mapList'i Enum isimleriyle doldurur
            mapList = Enum.GetNames(typeof(SessionMapName));
            DefaultValues();
        }

        private void DefaultValues()
        {
            _selectedMapName = mapList[0];
            selectedMapNameText.text = _selectedMapName.ToString();
            selectedMaSprite.sprite = mapImageList[0];
        }

        private void Next()
        {
            if (mapList == null || mapList.Length == 0)
                return;

            if (mapImageList == null || mapImageList.Length == 0)
                return;

            int safeLength = Mathf.Min(mapList.Length, mapImageList.Length);

            _selectedMapCountIndex = (_selectedMapCountIndex + 1) % safeLength;

            _selectedMapName = mapList[_selectedMapCountIndex];
            selectedMapNameText.text = _selectedMapName.ToString();
            selectedMaSprite.sprite = mapImageList[_selectedMapCountIndex];

            SetMap();
        }


        private void Previous()
        {
            _selectedMapCountIndex--;
            if (_selectedMapCountIndex < 0)
            {
                _selectedMapCountIndex += mapList.Length;
            }

            _selectedMapName = mapList[_selectedMapCountIndex];
            selectedMapNameText.text = _selectedMapName.ToString();
            selectedMaSprite.sprite = mapImageList[_selectedMapCountIndex];

            SetMap();
        }

        private void SetMap()
        {
            var mNameUpper = _selectedMapName.ToUpperInvariant();
            var mName = mNameUpper.Replace(' ', '_');

            GlobalManager.Instance.NetworkRunnerHandler.MapName = Enum.Parse<SessionMapName>(mName);
        }
    }
}