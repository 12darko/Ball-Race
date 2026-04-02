using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator
{
    public class SpectatorCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset      = new(0, 6, -8);
        [SerializeField] private float   followSpeed = 5f;
        [SerializeField] private float   rotateSpeed = 3f;

        public Transform CurrentTarget { get; private set; }

        private void Awake() => gameObject.SetActive(false);

        private void Update() => HandleInput();

        private void LateUpdate()
        {
            if (CurrentTarget == null) return;

            var desiredPos = CurrentTarget.position + CurrentTarget.rotation * offset;
            transform.position = Vector3.Lerp(
                transform.position, desiredPos, followSpeed * Time.deltaTime);

            var lookDir = CurrentTarget.position - transform.position;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookDir),
                    rotateSpeed * Time.deltaTime);
            }
        }

        public void SetTarget(Transform target) => CurrentTarget = target;

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                SpectatorManager.Instance?.WatchNextPlayer();

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                SpectatorManager.Instance?.WatchPreviousPlayer();
        }
    }
}