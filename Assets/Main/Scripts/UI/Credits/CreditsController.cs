using System;
using Assets._Main.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Main.Scripts.UI.Credits
{
    public class CreditsController : MonoBehaviour
    {

        [Header("Components")] [SerializeField]
        private MenuController menuController;

        [SerializeField] private GameObject creditsObject;
        
        [SerializeField] private Button backButton;


        private void Start()
        {
            backButton.onClick.AddListener(() => ObjectController.SetActiveEffectController(menuController.MenuObject, creditsObject, .65f));

        }
    }
}