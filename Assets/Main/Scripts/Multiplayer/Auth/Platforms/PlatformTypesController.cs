using System;
using EMA.Scripts.PatternClasses;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.UnityCore.Auth.Platforms
{
    public class PlatformTypesController : MonoSingleton<PlatformTypesController>
    {
        [SerializeField] private LoginPlatformType platformType;

        public LoginPlatformType PlatformType => platformType;
 
        
    }
}