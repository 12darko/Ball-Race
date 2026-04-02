using _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList;
using _Main.Scripts.Multiplayer.Singleton;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.FusionCore.Lobby.Room
{
    public class NetworkRoomList : MonoBehaviour
    {
        [SerializeField] private GameObject sessionInfoSlider;
        [SerializeField] private TMP_Text sessionLookingText;
        [SerializeField] private TMP_Text sessionNotFoundText;

        [SerializeField] private GameObject sessionRoomItemListPrefab;

        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Button createSessionButton;

        public void ClearList()
        {
            foreach (Transform child in gridLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }

            sessionLookingText.gameObject.SetActive(false);
        }

        public void AddToList(SessionInfo sessionInfo)
        { 
            sessionInfoSlider.gameObject.SetActive(false);
            var item = Instantiate(sessionRoomItemListPrefab, gridLayoutGroup.transform)
                .GetComponent<NetworkRoomItem>();
            item.SetInformationSession(sessionInfo);

            createSessionButton.enabled = true;
            item.OnJoinSession += NetworkRoomItem_OnJoinSession;
        }

        private void NetworkRoomItem_OnJoinSession(SessionInfo sessionInfo)
        {
            RoomListManager.Instance.JoinSession(sessionInfo);
        }

        public void OnSessionNotFound()
        {
            ClearList();
            
            sessionNotFoundText.gameObject.SetActive(true);
            sessionLookingText.gameObject.SetActive(false);
            createSessionButton.enabled = true;
            sessionInfoSlider.gameObject.SetActive(false);
        }

        public void OnSessionLookingForGame()
        {
            ClearList();
            sessionNotFoundText.gameObject.SetActive(false);
            sessionInfoSlider.gameObject.SetActive(true);
            sessionLookingText.text = "Looking For Game Session";
            sessionLookingText.gameObject.SetActive(true);
            createSessionButton.enabled = false;
        }
        
    }
}