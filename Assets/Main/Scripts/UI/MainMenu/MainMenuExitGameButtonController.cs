using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public class MainMenuExitGameButtonController : MonoBehaviour
    {
        [SerializeField] private Button exitButton;

        private void Start()
        {
            exitButton.onClick.AddListener(Exit);
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}