using Unity.Cinemachine;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator
{
    public class SpectatorCinemachine : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera vcam;

        public Transform CurrentTarget { get; private set; }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetTarget(Transform target)
        {
            CurrentTarget    = target;
            vcam.Follow      = target;
            vcam.LookAt      = target;
        }

        private void Update() => HandleInput();

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                SpectatorManager.Instance?.WatchNextPlayer();

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                SpectatorManager.Instance?.WatchPreviousPlayer();
        }
    }
}
 