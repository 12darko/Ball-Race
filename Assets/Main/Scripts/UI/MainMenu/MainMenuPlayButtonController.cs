using System;
using _Main.Scripts.Multiplayer.FusionCore.Lobby.Room;
using _Main.Scripts.Multiplayer.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public class MainMenuPlayButtonController : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Transform lobbyCanvas;
        [SerializeField] private Transform menuCanvas;
        [SerializeField] private NetworkRoomList networkRoomList;
        

        private void Start()
        {
            playButton.onClick.AddListener(Lobby);
        }

        private void Lobby()
        {
            MainMenuCanvasChanger.ChangeCanvas(lobbyCanvas, menuCanvas, false);
            GlobalManager.Instance.NetworkRunnerHandler.JoinLobby();
            networkRoomList.OnSessionLookingForGame();
        }
    }
}