using UnityEngine;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public static class MainMenuCanvasChanger
    {
        public static void ChangeCanvas(Transform openCanvas, Transform closeCanvas, bool isShowCloseCanvas)
        {
            if (isShowCloseCanvas)
            {
                openCanvas.gameObject.SetActive(true);
                closeCanvas.gameObject.SetActive(true);
            }
            else
            {
                openCanvas.gameObject.SetActive(true);
                closeCanvas.gameObject.SetActive(false);
            }
        }
    }
}