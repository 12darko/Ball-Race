using System;
using _Main.Scripts.Multiplayer.Multiplayer.Obstacles;
using UnityEngine;

namespace Player
{
    public class NetworkPlayerObstacleDetector : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerRb;
        
        private void OnCollisionEnter(Collision collision)
        {
            var obstacle = collision.collider.GetComponentInParent<Obstacle>();
            if (obstacle)
            {
                var pushDirection = collision.contacts[0].point - transform.position;
                pushDirection.y = 0; // Objeyi sadece yatay düzlemde it (isteğe bağlı)
                playerRb.AddForce(pushDirection.normalized * obstacle.obstacleForce , ForceMode.Impulse);
            }
        }
    }
}