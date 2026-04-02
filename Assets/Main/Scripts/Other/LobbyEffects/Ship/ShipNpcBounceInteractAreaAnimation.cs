using UnityEngine;

namespace Main.Scripts.Other.LobbyEffects.Ship
{
    public class ShipNpcBounceInteractAreaAnimation : MonoBehaviour
    {   [SerializeField] private BouncingObject ball;
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ship"))
            {
                 ball.StartBouncing(); // Oyuncu yaklaştığında başlat
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ship"))
            {
                ball.StopBouncing(); // Oyuncu uzaklaştığında durdur
            }
        }
    }
}