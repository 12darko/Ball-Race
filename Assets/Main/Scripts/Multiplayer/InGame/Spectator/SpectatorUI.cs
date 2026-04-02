using TMPro;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator
{
    public class SpectatorUI : MonoBehaviour
    {
        public static SpectatorUI Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI watchingLabel;
        [SerializeField] private TextMeshProUGUI navigationHint;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void ShowTargetName(string playerName)
        {
            if (watchingLabel)  watchingLabel.text  = $"Watching: {playerName}";
            if (navigationHint) navigationHint.text = "← → switch player";
        }
    }
}