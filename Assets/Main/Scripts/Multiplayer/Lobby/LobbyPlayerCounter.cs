using System;
using _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.Multiplayer.Lobby
{
    public class LobbyPlayerCounter : MonoBehaviour
    {

        [SerializeField] private TMP_Text selectedPlayerCountText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private int[] countList;
        [SerializeField] private int selectedPlayerCountIndex;
        [SerializeField] private int selectedPlayerCount;

        private void Start()
        {
            nextButton.onClick.AddListener(Next);
            previousButton.onClick.AddListener(Previous);
            DefaultValues();
        }
        
        private void DefaultValues()
        {
            selectedPlayerCount = countList[0];
            selectedPlayerCountText.text = selectedPlayerCount.ToString();
        }
        private void Next()
        {
            selectedPlayerCountIndex = (selectedPlayerCountIndex + 1)% countList.Length;
            selectedPlayerCount = countList[selectedPlayerCountIndex];
            selectedPlayerCountText.text = selectedPlayerCount.ToString();
            
            RoomListManager.Instance.SessionMaxPlayerCount = selectedPlayerCount;

        }

        private void Previous()
        {
            selectedPlayerCountIndex--;
            if (selectedPlayerCountIndex < 0)
            {
                selectedPlayerCountIndex += countList.Length;
            }
            selectedPlayerCount = countList[selectedPlayerCountIndex];
            selectedPlayerCountText.text = selectedPlayerCount.ToString();
            
            RoomListManager.Instance.SessionMaxPlayerCount = selectedPlayerCount;

        }
    }
}