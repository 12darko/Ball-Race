using _Main.Scripts.Multiplayer.Player.NetworkInput;
using Fusion;
using UnityEngine;

namespace Player
{
    public class NetworkPlayerCameraRotate : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform lookAt;
        [SerializeField] private NetworkPlayers networkPlayers;

        [Header("Camera Settings")]
        [SerializeField]
        public float smoothTime = 15f;
        [SerializeField] private Vector2 verticalClamp = new Vector2(-40, 20);

        [SerializeField, Networked] public Vector3 CameraForwardPos { get; set; }
        [SerializeField, Networked] public Vector3 CameraLookPosition { get; set; }
// 1. Field olarak ekle (private bool zaten varsa değiştir)
        private bool _mouseLocked = false;
 
// 2. Property — mevcut _currentX'e dışarıdan erişim
        public float CurrentYaw => _currentX;
        // ✅ Tam kamera yönü - dikey dahil, face manager kullanır
        [Networked] public Vector3 NetworkedCameraFullForward { get; set; }

        public float SmoothTime
        {
            get => smoothTime;
            set => smoothTime = value;
        }

        public Transform LookAt => lookAt;
 
// 3. Dışarıdan yaw set etmek için
        public void SetYaw(float yaw)
        {
            _currentX = yaw;
        }
 
// 4. Mouse kontrolünü kilitle/aç
        public void SetMouseLocked(bool locked)
        {
            _mouseLocked = locked;
        }
 
 
        private float _currentX;
        private float _currentY;
        private Quaternion _targetRotation;
        private bool _initialized;
        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkPlayerInput playerInput))
            {
                CameraForwardPos = playerInput.NetworkCameraForwardPosition;
                CameraLookPosition = playerInput.NetworkCameraLookPosition;
                // ✅ Tam forward sync - yukarı aşağı dahil
                NetworkedCameraFullForward = playerInput.NetworkAimForward;
            }
        }

      

        public override void Render()
        {
            if (!Object.HasInputAuthority) return;

            // ✅ İlk render'da başlangıç açısını sıfırla
            if (!_initialized)
            {
                _currentX = transform.rotation.eulerAngles.y;
                _currentY = 0f;
                _initialized = true;
            }

            HandleMouseInput();
            ApplyRotation();
        }

        private void HandleMouseInput()
        {    
            if (_mouseLocked) return; // ← bunu ekle
            float sensitivity = networkPlayers.networkPlayersManager.NetworkPlayerStats.NetworkPlayerCameraRotationSpeed;

            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime * 50f;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime * 50f;

            _currentX += mouseX;
            _currentY -= mouseY;
            _currentY = Mathf.Clamp(_currentY, verticalClamp.x, verticalClamp.y);
        }

        private void ApplyRotation()
        {
            _targetRotation = Quaternion.Euler(_currentY, _currentX, 0);
            lookAt.rotation = Quaternion.Slerp(lookAt.rotation, _targetRotation, smoothTime * Time.deltaTime);
        }
    }
}