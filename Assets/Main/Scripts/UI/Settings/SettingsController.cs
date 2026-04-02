using System;
using Assets._Main.Scripts.UI.MainMenu;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Main.Scripts.UI.Settings
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private MenuController menuController;
        [SerializeField] private GameObject settingsObject;

        [SerializeField] private Button backButton;

        private void Start()
        {
            backButton.onClick.AddListener(() => ObjectController.SetActiveEffectController(menuController.MenuObject, settingsObject, .65f));
        }

  
    }
}