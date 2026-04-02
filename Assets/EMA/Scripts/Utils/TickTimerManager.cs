using Fusion;
using UnityEngine;

namespace _Main.EMA.Scripts.Utils
{
    public class TickTimerManager : NetworkBehaviour
    {
        [Networked] private TickTimer networkedTickTimer { get; set; }

        public void StartTickTimer(float second)
        {
            networkedTickTimer = TickTimer.CreateFromSeconds(Runner, second);
        }

        public bool CheckTimerExpired()
        {
            return networkedTickTimer.Expired(Runner);
        }
        
        public bool CheckTimerExpiredOrNotRunning()
        {
            return networkedTickTimer.ExpiredOrNotRunning(Runner);
        }

        public void DebugRemainingTime()
        {
            Debug.Log(networkedTickTimer.RemainingTime(Runner));
        }

        public void ResetTickTimer()
        {
            networkedTickTimer = TickTimer.None;
        }
    }
}