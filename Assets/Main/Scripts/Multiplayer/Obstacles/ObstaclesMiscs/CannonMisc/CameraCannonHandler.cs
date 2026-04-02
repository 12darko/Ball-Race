using System.Collections;
using _Main.Scripts.Multiplayer.Multiplayer.Obstacles;
using UnityEngine;

namespace Player
{
   /// <summary>
    /// NetworkPlayerCameraRotate objesine ekle.
    /// Cannon'a girince topun fırlatma yönüne smooth döner.
    /// Fırlatma sonrası kısa bekleyip mouse kontrolünü geri verir.
    /// </summary>
    public class CameraCannonHandler : MonoBehaviour
    {
        [SerializeField] private NetworkPlayerCameraRotate cameraRotate;
 
        [Tooltip("Cannon yönüne smooth dönüş süresi (sn)")]
        [SerializeField] private float alignDuration = 0.5f;
 
        [Tooltip("Fırlatma sonrası mouse kontrolü geri gelene kadar bekleme (sn)")]
        [SerializeField] private float mouseRestoreDelay = 0.8f;
 
        private bool      _cannonActive   = false;
        private Coroutine _alignCoroutine = null;
        private Coroutine _restoreCoroutine = null;
 
        private void Awake()
        {
            if (cameraRotate == null)
                cameraRotate = GetComponent<NetworkPlayerCameraRotate>();
        }
 
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
 
        // ─────────────────────────────────────────────────────────
        // Cannon'a girince — fırlatma yönüne smooth dön
        // ─────────────────────────────────────────────────────────
 
        private void HandleCannonFired()
        {
            if (cameraRotate == null) return;
            _cannonActive = true;
 
            // Mouse kontrolünü kilitle
            cameraRotate.SetMouseLocked(true);
 
            // Cannon'ın fırlatma yönünü bul — CannonObstacle'ın transform.right'ı
            CannonObstacle cannon = FindObjectOfType<CannonObstacle>();
            if (cannon == null) return;
 
            // Cannon yönü: transform.right (fırlatma yönü)
            Vector3 cannonDir = cannon.transform.right;
            cannonDir.y = 0f;
 
            if (cannonDir == Vector3.zero) return;
 
            float targetYaw = Quaternion.LookRotation(cannonDir).eulerAngles.y;
 
            if (_alignCoroutine != null) StopCoroutine(_alignCoroutine);
            _alignCoroutine = StartCoroutine(SmoothAlignCamera(targetYaw));
        }
 
        // ─────────────────────────────────────────────────────────
        // Cannon hazır — mouse kontrolünü geri ver
        // ─────────────────────────────────────────────────────────
 
        private void HandleCannonReady()
        {
            if (!_cannonActive) return;
 
            if (_restoreCoroutine != null) StopCoroutine(_restoreCoroutine);
            _restoreCoroutine = StartCoroutine(RestoreMouseAfterDelay());
        }
 
        // ─────────────────────────────────────────────────────────
        // Coroutines
        // ─────────────────────────────────────────────────────────
 
        private IEnumerator SmoothAlignCamera(float targetYaw)
        {
            float startYaw   = cameraRotate.CurrentYaw;
            float elapsed    = 0f;
 
            while (elapsed < alignDuration)
            {
                float t   = elapsed / alignDuration;
                float eased = t * t * (3f - 2f * t); // smoothstep
 
                float yaw = Mathf.LerpAngle(startYaw, targetYaw, eased);
                cameraRotate.SetYaw(yaw);
 
                elapsed += Time.deltaTime;
                yield return null;
            }
 
            cameraRotate.SetYaw(targetYaw);
        }
 
        private IEnumerator RestoreMouseAfterDelay()
        {
            yield return new WaitForSeconds(mouseRestoreDelay);
            _cannonActive = false;
            cameraRotate.SetMouseLocked(false);
        }
    }
}