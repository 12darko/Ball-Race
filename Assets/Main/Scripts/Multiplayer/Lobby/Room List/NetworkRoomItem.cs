using System;
using _Main.Scripts.Multiplayer.Runners;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Multiplayer.FusionCore.Lobby.Room
{
    public class NetworkRoomItem : MonoBehaviour
    {
        [SerializeField] private Sprite sessionIsNormalButtonSprite;
        [SerializeField] private Sprite sessionIsFullButtonSprite;

        [SerializeField] private GameObject sessionDetails;

        [SerializeField] private TMP_Text sessionNameTxt;
        [SerializeField] private TMP_Text sessionMapText;
        [SerializeField] private TMP_Text playerCountTxt;
        [SerializeField] private Button joinBtn;

        [SerializeField] private SessionMapName mapName;

        private SessionInfo _sessionInfo;

        public event Action<SessionInfo> OnJoinSession;

        private void Start()
        {
            joinBtn.onClick.AddListener(JoinSession);
        }

        public void SetInformationSession(SessionInfo sessionInfo)
        {
            _sessionInfo = sessionInfo;

            //session Name üzerinde perdeleme yaptık random session oluştururken kendi adını göstermemek için parçalara bölüp ismi değiştik
            if (sessionInfo.Name.Contains("-"))
            {
                var sessionName = "RUSH " + Random.Range(0, 100000) + "  " + sessionInfo.Name;
                var sessionNameSplit = sessionName.Split("  ");
                sessionNameTxt.text = sessionNameSplit[0].ToUpper();
            }
            else
            {
                sessionNameTxt.text = sessionInfo.Name.ToUpper();
            }

            mapName = (SessionMapName)Enum.Parse(typeof(SessionMapName), sessionInfo.Properties["GameMap"]);
            var renameMap = mapName.ToString().Replace('_', ' ');
            sessionMapText.text = renameMap.ToUpper();
            playerCountTxt.text = sessionInfo.PlayerCount.ToString().ToUpper() + " / " +
                                  sessionInfo.MaxPlayers.ToString().ToUpper();

            if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
            {
                joinBtn.enabled = false;
                joinBtn.GetComponent<Image>().sprite = sessionIsFullButtonSprite;
                joinBtn.GetComponentInChildren<TMP_Text>().text = "FULL";
            }
            else
            {
                joinBtn.enabled = true;
                joinBtn.GetComponent<Image>().sprite = sessionIsNormalButtonSprite;
                joinBtn.GetComponentInChildren<TMP_Text>().text = "JOIN";
            }
        }

        private void JoinSession()
        {
            OnJoinSession?.Invoke(_sessionInfo);
        }
    }
}