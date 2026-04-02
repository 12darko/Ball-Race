using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.MainMenu
{
    public class MainMenuBugReportButtonController : MonoBehaviour
    {
        [SerializeField] private Button bugReport;
        [SerializeField] private Transform bugCanvas;
        [SerializeField] private Transform menuCanvas;


        private void Start()
        {
            bugReport.onClick.AddListener(Report);
        }

        private void Report()
        {
            MainMenuCanvasChanger.ChangeCanvas(bugCanvas, menuCanvas, true);
        }
        
    }
}