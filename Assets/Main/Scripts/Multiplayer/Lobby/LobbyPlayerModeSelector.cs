using System;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using _Main.Scripts.Multiplayer.Runners;
using _Main.Scripts.Multiplayer.Singleton;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.Multiplayer.Lobby
{
    public class LobbyPlayerModeSelector : MonoBehaviour
    {
        [SerializeField] private TMP_Text selectedModeNameText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private string[] modeList;
        [SerializeField] private InGameMode inGameMode;
        private int _selectedModeCountIndex;
        private string _selectedModeName;

        public InGameMode InGameMode => inGameMode;

        private void Start()
        {
            nextButton.onClick.AddListener(Next);
            previousButton.onClick.AddListener(Previous);
            
            modeList = Enum.GetNames(typeof(InGameMode));
            DefaultValues();
        }
        private void DefaultValues()
        {
            _selectedModeName = modeList[0];
            
            selectedModeNameText.text = _selectedModeName.Replace("_", " "); //Ui için isimlerin tiresini kaldır
         }
        private void Next()
        {
            if (modeList == null || modeList.Length == 0)
                return;

            _selectedModeCountIndex = (_selectedModeCountIndex + 1) % modeList.Length;

            _selectedModeName = modeList[_selectedModeCountIndex];

            selectedModeNameText.text = _selectedModeName
                .Replace("_", " ")
                .Replace("Mode", "");

            SetMode();
        }

        private void Previous()
        {
            _selectedModeCountIndex--;
            if (_selectedModeCountIndex < 0)
            {
                _selectedModeCountIndex += modeList.Length;
            }
            _selectedModeName = modeList[_selectedModeCountIndex];
            selectedModeNameText.text = _selectedModeName.Replace("_", " ");//Ui için isimlerin tiresini kaldır
             
            SetMode();
        }
        
        private void SetMode()
        {
            var mNameUpper = _selectedModeName;
            var mName = mNameUpper.Replace(' ', '_');
 
            inGameMode = Enum.Parse<InGameMode>(mName.ToString());
            GlobalManager.Instance.NetworkRunnerHandler.ModeName = inGameMode;
        }
    }
}