using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Main.Scripts.UI.MainMenu
{
    public class MenuExitButtons : MonoBehaviour
    {
        [SerializeField] private Button exitButtons;


        private void Start()
        {
            exitButtons.onClick.AddListener(ExitGame);
        }

        private void ExitGame()
        {
            Debug.Log("Oyun kapatılıyor...");
            Application.Quit();
        }
    }
}