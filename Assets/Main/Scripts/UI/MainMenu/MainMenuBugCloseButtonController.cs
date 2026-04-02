using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public class MainMenuBugCloseButtonController : MonoBehaviour
    {
        [SerializeField] private Button closeReportButton;
        [SerializeField] private Transform bugCanvas;
        [SerializeField] private Transform menuCanvas;


        private void Start()
        {
            closeReportButton.onClick.AddListener(Close);
        }

        private void Close()
        {
            MainMenuCanvasChanger.ChangeCanvas(menuCanvas, bugCanvas, false);
        }
    }
}