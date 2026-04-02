using EMA.Scripts.PatternClasses;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList.RoomListPageGameSettings
{
    public class RoomListGamePanelSettings : MonoSingleton<RoomListGamePanelSettings>
    {
        [SerializeField] private TMP_Text gameSettingsModeText;
        [SerializeField] private TMP_Text gameSettingsMatchTimeText;
        [SerializeField] private TMP_Text gameSettingsMatchScoreText;

        [SerializeField] private GameObject gameSettingsTopMatchScoreGameObject;
        
        [SerializeField] private GameObject gameSettingsMatchTimeSettingsGameObject;
        [SerializeField] private GameObject gameSettingsMatchScoreSettingsGameObject;

        public TMP_Text GameSettingsModeText
        {
            get => gameSettingsModeText;
            set => gameSettingsModeText = value;
        }

        public TMP_Text GameSettingsMatchTimeText
        {
            get => gameSettingsMatchTimeText;
            set => gameSettingsMatchTimeText = value;
        }

        public TMP_Text GameSettingsMatchScoreText
        {
            get => gameSettingsMatchScoreText;
            set => gameSettingsMatchScoreText = value;
        }


        public GameObject GameSettingsTopMatchScoreGameObject
        {
            get => gameSettingsTopMatchScoreGameObject;
            set => gameSettingsTopMatchScoreGameObject = value;
        }

        public GameObject GameSettingsMatchTimeSettingsGameObject
        {
            get => gameSettingsMatchTimeSettingsGameObject;
            set => gameSettingsMatchTimeSettingsGameObject = value;
        }

        public GameObject GameSettingsMatchScoreSettingsGameObject
        {
            get => gameSettingsMatchScoreSettingsGameObject;
            set => gameSettingsMatchScoreSettingsGameObject = value;
        }
    }
}