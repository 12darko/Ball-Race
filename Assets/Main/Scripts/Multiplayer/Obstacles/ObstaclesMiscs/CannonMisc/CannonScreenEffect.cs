using System.Collections;
using _Main.Scripts.Multiplayer.Multiplayer.Obstacles;
using UnityEngine;

namespace Player
{
    public class CannonScreenEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem speedLinesParticle;
 
        private void OnEnable()
        {
            CannonObstacle.OnCannonFired += HandleCannonFired;
            CannonObstacle.OnCannonReady += HandleCannonReady;
        }
 
        private void OnDisable()
        {
            CannonObstacle.OnCannonFired -= HandleCannonFired;
            CannonObstacle.OnCannonReady -= HandleCannonReady;
        }
 
        private void HandleCannonFired()
        {
            if (speedLinesParticle == null) return;
            speedLinesParticle.Play();
        }
 
        private void HandleCannonReady()
        {
            if (speedLinesParticle == null) return;
            speedLinesParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}