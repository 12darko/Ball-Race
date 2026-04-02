using System;
 
using DG.Tweening;
using EMA.Scripts.PatternClasses;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets._Main.Scripts.UI.MainMenu
{
    public class MenuController : MonoSingleton<MenuController>
    {
        [FormerlySerializedAs("menuObjects")]
        [Header("Game Objects")]

        #region Game Objects

        [SerializeField]
        private GameObject menuObject;
        [SerializeField] private GameObject settingsObject;
        [SerializeField] private GameObject creditsObject;
        [SerializeField] private GameObject mainLobbyObjects;
        [SerializeField] private GameObject modeGameObject;


        #endregion
        [Header("Buttons")]
        #region Buttons

        [SerializeField] private Button playBtn;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;

        #endregion

        private Vector2 _startPosMenuObject;
        private Vector2 _startPosSettingsObject;

        #region Props

        public GameObject MenuObject
        {
            get => menuObject;
            set => menuObject = value;
        }

        public GameObject MainLobbyObjects
        {
            get => mainLobbyObjects;
            set => mainLobbyObjects = value;
        }

        public Vector2 StartPosSettingsObject
        {
            get => _startPosSettingsObject;
            set => _startPosSettingsObject = value;
        }

        public GameObject ModeGameObject
        {
            get => modeGameObject;
            set => modeGameObject = value;
        }

        #endregion

     

        private void Start()
        {  
            DOTween.defaultAutoKill = false;
            ObjectController.SetActiveEffectController(menuObject, null, .65f);
            
            settingsButton.onClick.AddListener(() => ObjectController.SetActiveEffectController(settingsObject, menuObject, .65f));
            playBtn.onClick.AddListener(() => ObjectController.SetActiveEffectController(modeGameObject, menuObject, .65f));
            creditsButton.onClick.AddListener(() => ObjectController.SetActiveEffectController(creditsObject, menuObject, .65f));

        }
 
     
    }
}