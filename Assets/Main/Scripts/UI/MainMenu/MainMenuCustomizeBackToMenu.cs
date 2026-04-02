using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public class MainMenuCustomizeBackToMenu : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Transform customizeCanvas;
        [SerializeField] private Transform menuCanvas;
        private void Start()
        {
            backButton.onClick.AddListener(Back);
        }

        private void Back()
        {
             MainMenuCanvasChanger.ChangeCanvas(menuCanvas, customizeCanvas, false);
        }
    }
}