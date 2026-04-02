using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.UI
{
    public class LobbyUIManager : MonoBehaviour
    {
        [Header("Lobby Info")]
        [SerializeField] private TMP_InputField lobbyNameText;
        [SerializeField] private TMP_InputField playerCountText;
        [SerializeField] private TMP_InputField gameModeText;
        [SerializeField] private TMP_InputField gameMapText;
        [SerializeField] private TMP_Text countdownText;

        [Header("Buttons")]
        [SerializeField] private Button readyToggleButton;
        [SerializeField] private Button backButton;

        [Header("Ready Button Visuals")]
        [SerializeField] private TMP_Text readyButtonText;
        [SerializeField] private Image readyButtonImage;
        [SerializeField] private Sprite readySprite;
        [SerializeField] private Sprite notReadySprite;

        [Header("Countdown")]
        [SerializeField] private GameObject countdownPanel;

        private LobbyManager _lobbyManager;
        private bool _isReady;

        private void Start()
        {
            if (readyToggleButton != null)
                readyToggleButton.onClick.AddListener(OnReadyToggleClicked);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);

            if (countdownPanel != null)
                countdownPanel.SetActive(false);

            UpdateReadyButton(false); // default NOT READY
        }

        public void Initialize(LobbyManager lobbyManager)
        {
            _lobbyManager = lobbyManager;
        }

        public void UpdateLobbyInfo(string lobbyName, int currentPlayers, int maxPlayers, string gameMode, string gameMap)
        {
            if (lobbyNameText != null)
                lobbyNameText.text = lobbyName;

            if (playerCountText != null)
                playerCountText.text = $"{currentPlayers}/{maxPlayers}";

            if (gameModeText != null)
                gameModeText.text = gameMode.Replace("_", " ");

            if (gameMapText != null)
                gameMapText.text = gameMap;
        }

        public void ShowCountdown(int seconds)
        {
            if (countdownPanel != null)
                countdownPanel.SetActive(true);

            if (countdownText != null)
                countdownText.text = $"STARTING IN {seconds} SECONDS";
        }

        public void HideCountdown()
        {
            if (countdownPanel != null)
                countdownPanel.SetActive(false);
        }

        /// <summary>
        /// Network’ten gelen gerçek ready durumuna göre butonu günceller
        /// </summary>
        public void UpdateReadyButton(bool isReady)
        {
            _isReady = isReady;

            if (readyButtonText != null)
                readyButtonText.text = isReady ? "READY" : "NOT READY";

            if (readyButtonImage != null)
            {
                readyButtonImage.sprite = isReady ? readySprite : notReadySprite;
            }
        }

        private void OnReadyToggleClicked()
        {
            _isReady = !_isReady;
            _lobbyManager?.SetLocalPlayerReady(_isReady);
            UpdateReadyButton(_isReady);
        }

        private void OnBackClicked()
        {
            _lobbyManager?.LeaveLobby();
        }

        private void OnDestroy()
        {
            if (readyToggleButton != null)
                readyToggleButton.onClick.RemoveListener(OnReadyToggleClicked);

            if (backButton != null)
                backButton.onClick.RemoveListener(OnBackClicked);
        }
    }
}
