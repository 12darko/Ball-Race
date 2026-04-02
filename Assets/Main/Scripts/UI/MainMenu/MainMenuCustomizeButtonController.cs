using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public class MainMenuCustomizeButtonController : MonoBehaviour
    {
        [SerializeField] private Button customizeButton;
        [SerializeField] private Transform menuCanvas;
        [SerializeField] private Transform customizeCanvas;


        private void Start()
        {
            customizeButton.onClick.AddListener(Customize);
        }

        private void Customize()
        {
            MainMenuCanvasChanger.ChangeCanvas(customizeCanvas, menuCanvas, false);
        }
    }
}