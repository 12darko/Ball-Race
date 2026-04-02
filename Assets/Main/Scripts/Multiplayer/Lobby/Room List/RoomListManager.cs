using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList.RoomListPageGameSettings;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using _Main.Scripts.Multiplayer.PopUp;
using _Main.Scripts.Multiplayer.Runners;
using _Main.Scripts.Multiplayer.Singleton;
using EMA.Scripts.PatternClasses;
using Fusion;
using Main.Scripts.Multiplayer.Lobby;
using Multiplayer.Player;
using Other;
using UnityEngine;
using GameMode = Fusion.GameMode;

namespace _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList
{
    public sealed class RoomListManager : MonoSingleton<RoomListManager>
    {
        [SerializeField] private RoomListData roomListData;
        [SerializeField] private LobbyPlayerModeSelector playerModeSelector;

        [SerializeField] private string sessionName;
        [SerializeField] private bool sessionIsOpen;
        [SerializeField] private bool sessionIsVisible;
        [SerializeField] private int sessionMaxPlayerCount;
        [SerializeField] private int matchTime;
        [SerializeField] private int matchScoreCount;
        [SerializeField] private SessionMapName mapName;


        public int SessionMaxPlayerCount
        {
            get => sessionMaxPlayerCount;
            set => sessionMaxPlayerCount = value;
        }

        private void Start()
        {
            DefaultSessionValues();

            roomListData.RoomNameTxtFıeld.onValueChanged.AddListener(delegate
            {
                sessionName = roomListData.RoomNameTxtFıeld.text;
            });

            roomListData.CreatePublicSessionButton.onClick.AddListener(CreateSession);
        }

        private void DefaultSessionValues()
        {
            //Varsayılan Oda Kurma Değerleri 
            sessionMaxPlayerCount = 2;
            sessionIsOpen = true;
            sessionIsVisible = true;
        }

        private void CreateSession()
        {
            if (string.IsNullOrEmpty(sessionName))
            {
                //TODO     GlobalManager.Instance.PopUpManager.OpenPopUp("İsim boş bırakılamaz", PopUpTitleInfo.Error, PopUpType.Info);
                return;
            }

            if (sessionName.Length < 5)
            {
                //TODO   GlobalManager.Instance.PopUpManager.OpenPopUp("İsim 5 karakterden küçük olamaz", PopUpTitleInfo.Error, PopUpType.Info);
                return;
            }

            GlobalManager.Instance.NetworkRunnerHandler.StartGame(GameMode.Host,
                MiscGlobal.GetGameMode(playerModeSelector.InGameMode), sessionName,
                sessionIsVisible, sessionIsOpen, sessionMaxPlayerCount,
                MiscGlobal.GetGameModeIsTeam(playerModeSelector.InGameMode), "Oyun Oluşturuluyor...",
                "Oyun Oluşturulamadı !!!");
            roomListData.CreatePublicSessionButton.enabled = false;
            //TODO sTEAM VERSİYONU GELCEK İNVİTE
            /*   GameInviteListMyGroup.ListGroups(PlayFabEntityData.Instance.EntityKey);
               GameInviteCreateGroup.CreateGroup(sessionName);
               PlayFabMessages.OnEventCreateGroupResponse += (sender, response) =>
               {
                   GameInviteSetGroupObjectData.SetGroupObjectData(
                       PlayFabEntityData.EntityKeyMakerDataModels(response.Group.Id), sessionName, sessionIsVisible,
                       sessionIsOpen, sessionMaxPlayerCount);
               };*/
            // GlobalManager.Instance.menuManager.MenuData.BackButton.GetComponent<NetworkLobbyBack>().EntityKeys.Add(PlayFabEntityData.Instance.EntityKey);
            PlayerData.Instance.PlayerAuthority = 1;
        }

        public void JoinSession(SessionInfo sessionInfo)
        {
            Debug.Log((InGameMode)Enum.Parse(typeof(InGameMode), sessionInfo.Properties["GameMode"]));
            GlobalManager.Instance.NetworkRunnerHandler.StartGame(GameMode.Client,
                MiscGlobal.GetGameMode((InGameMode)Enum.Parse(typeof(InGameMode), sessionInfo.Properties["GameMode"])),
                sessionInfo.Name,
                sessionInfo.IsVisible,
                sessionInfo.IsOpen,
                sessionInfo.MaxPlayers
                , MiscGlobal.GetGameModeIsTeam((InGameMode)Enum.Parse(typeof(InGameMode), sessionInfo.Properties["GameMode"])),
                "Oyuna Katılınıyor...",
                "Oyuna Katılınamadı !!!");

            PlayerData.Instance.PlayerAuthority = 0;


            //   GlobalManager.Instance.PopUpManager.OpenPopUp("Oyuna Katılınıyor", PopUpTitleInfo.Loading, PopUpType.Loader);
        }
    }
}