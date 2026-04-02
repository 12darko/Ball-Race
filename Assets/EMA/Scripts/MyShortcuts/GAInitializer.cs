/*using System;
using GameAnalyticsSDK;
using UnityEngine;

namespace EMA.MyShortcuts
{
    public class GAInitializer : MonoBehaviour
    {
        public static GAInitializer Instance;
        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
            }

            else {
                Destroy(this.gameObject);
                return;
            }
            
            DontDestroyOnLoad(this.gameObject);
            
            GameAnalytics.Initialize();
        }
    }
}*/
