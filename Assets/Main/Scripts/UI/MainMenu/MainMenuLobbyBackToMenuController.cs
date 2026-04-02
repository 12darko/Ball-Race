using System;
using _Main.Scripts.Multiplayer.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public class MainMenuLobbyBackToMenuController : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Transform lobbyCanvas;
        [SerializeField] private Transform menuCanvas;
        private void Start()
        {
            backButton.onClick.AddListener(Back);
        }

        private void Back()
        {
            GlobalManager.Instance.NetworkRunnerHandler.Disconnect();
            MainMenuCanvasChanger.ChangeCanvas(menuCanvas, lobbyCanvas, false);
        }
    }
}