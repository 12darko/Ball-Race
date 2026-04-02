using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList
{
    public class RoomListData : MonoBehaviour
    {
         
        [SerializeField] private TMP_InputField roomNameTxtField;
        [FormerlySerializedAs("createPublicSessionButton")] [FormerlySerializedAs("createSessionButton")] [SerializeField] private Button createPublicPublicSessionButton;
 
      
        public TMP_InputField RoomNameTxtFıeld
        {
            get => roomNameTxtField;
            set => roomNameTxtField = value;
        }

        public Button CreatePublicSessionButton
        {
            get => createPublicPublicSessionButton;
            set => createPublicPublicSessionButton = value;
        }
    }
}