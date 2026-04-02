using System;
using UnityEngine;

namespace _Main.Scripts.Multiplayer
{
    public class DDOL : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}