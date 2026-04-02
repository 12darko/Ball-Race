 
using _Main.Scripts.Multiplayer.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.FusionCore.StartManager.UI.RoomList
{
    public class RoomListBackToMenu : MonoBehaviour
    {  
        [Header("Components")]
        [SerializeField] private Button returnToMenuButton;
        
        private void Start()
        {
            returnToMenuButton.onClick.AddListener(() =>
            {
                GlobalManager.Instance.NetworkRunnerHandler.Disconnect();
               // MenuCanvasChanger.ChangeCanvas(    GlobalManager.Instance.menuManager.MenuData.MainMenuCanvas.transform,     GlobalManager.Instance.menuManager.MenuData.RoomListCanvas.transform, false);
              //  GlobalManager.Instance.menuManager.MenuData.MenuCanvas.gameObject.SetActive(true);

            });
        }
    }
}