using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.UI
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image readyIndicator;
        [SerializeField] private Color readyColor = Color.green;
        [SerializeField] private Color notReadyColor = Color.red;
        
        private PlayerRef _playerRef;
        
        public void Setup(string playerName, PlayerRef playerRef, bool isReady)
        {
            _playerRef = playerRef;
            UpdateUI(playerName, isReady);
        }
        
        public void UpdateUI(string playerName, bool isReady)
        {
            if (playerNameText != null)
                playerNameText.text = playerName;
            
            if (readyIndicator != null)
                readyIndicator.color = isReady ? readyColor : notReadyColor;
        }
        
        public PlayerRef GetPlayerRef() => _playerRef;
    }
}