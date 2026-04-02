using System;
using System.Collections.Generic;
using EMA.Scripts.PatternClasses;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize
{
    public class CustomizeManager : MonoSingleton<CustomizeManager>
    {
        [SerializeField] private GameObject selectedBall;
        [SerializeField] private GameObject selectedHat;
        [SerializeField] private GameObject selectedFace;
      
        [SerializeField] private int selectedTypeId;

        public int SelectedTypeId
        {
            set => selectedTypeId = value;
        }
    }
}