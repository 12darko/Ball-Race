using System;
using Assets._Main.Scripts.UI.MainMenu;
 
using Multiplayer.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Main.Scripts.UI.Lobby
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private MenuController menuController;
        [SerializeField] private Button backButton;

      /*  [SerializeField] private MLobbyList lobbyList;*/

        private void Awake()
        {
        }

        private void Start()
        {
         /*   backButton.onClick.AddListener(() =>
            {
                lobbyList = FindObjectOfType<MLobbyList>(false);
                if (lobbyList != null)
                {
                    lobbyList.ClearLobbyUI();
                }

                PlayerData.Instance.SetMode("");

                ObjectController.SetActiveEffectController(menuController.ModeGameObject,
                    menuController.MainLobbyObjects, .65f);
            });*/
        }
    }
}