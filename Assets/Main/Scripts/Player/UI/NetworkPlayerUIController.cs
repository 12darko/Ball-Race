 using Fusion;
 using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.Player
{
    public class NetworkPlayerUIController : NetworkBehaviour
    {
        [SerializeField] private GameObject playerUI;
        [SerializeField] private Slider healthBarSlider;
        public Slider HealthBarSlider => healthBarSlider;


    /*    public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                playerUI.gameObject.SetActive(true);
            }
        }*/
    }
}