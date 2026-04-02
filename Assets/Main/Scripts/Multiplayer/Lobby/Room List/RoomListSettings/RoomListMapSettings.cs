using System;
using _Main.Scripts.Multiplayer.Runners;
 using _Main.Scripts.Multiplayer.Singleton;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList.RoomListSettings
{
    public class RoomListMapSettings : MonoBehaviour
    {
        private void Start()
        {
         /*   RoomListManager.Instance.MapName = (SessionMapName)Enum.Parse(typeof(SessionMapName),   RoomListManager.Instance.RoomListData.RoomMapDropdown.options[  RoomListManager.Instance.RoomListData.RoomMapDropdown.value].text);
            GlobalManager.Instance.NetworkRunnerHandler.MapName =  RoomListManager.Instance.MapName;*/
        }
    }
}