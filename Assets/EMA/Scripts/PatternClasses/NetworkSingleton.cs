using Fusion;
using UnityEngine;

namespace EMA.Scripts.PatternClasses
{
    public abstract class NetworkSingleton<T> : NetworkBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            Instance = GetComponent<T>();
        }

    }
}