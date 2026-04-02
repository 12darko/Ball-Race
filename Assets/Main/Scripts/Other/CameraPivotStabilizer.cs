using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class CameraPivotStabilizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform pivot;
    [SerializeField] private MonoBehaviour groundChecker;
    [SerializeField] private NetworkRigidbody3D networkRigidbody;
    [SerializeField] private Rigidbody rb;

    [Header("Offsets")]
    [SerializeField] private Vector3 groundedOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private Vector3 airOffset     = new Vector3(0f, 1.8f, 0f);

    [Header("Smoothing")]
    [SerializeField] private float groundedPosSmooth = 0.05f;  // ✅ artırıldı
    [SerializeField] private float airPosSmooth      = 0.08f;  // ✅ artırıldı
    [SerializeField] private float snapDistance      = 5f;     // ✅ çok uzaksa direkt snap

    private Vector3 _vel;
    private bool    _pivotInitialized;

    private bool IsGrounded()
    {
        if (groundChecker == null) return true;
        try
        {
            var m = groundChecker.GetType().GetMethod("IsGrounded");
            return m != null && (bool)m.Invoke(groundChecker, null);
        }
        catch { return true; }
    }

    private Vector3 GetInterpolatedRootPos()
    {
        // ✅ Fusion'ın interpolate ettiği pozisyonu kullan — en smooth sonuç
        if (networkRigidbody != null && networkRigidbody.InterpolationTarget != null)
            return networkRigidbody.InterpolationTarget.position;

        if (rb != null)
            return rb.position; // Rigidbody.position fizik adımına sync'li

        return playerRoot.position;
    }

    private void LateUpdate()
    {
        if (playerRoot == null || pivot == null) return;

        Vector3 rootPos = GetInterpolatedRootPos();

        bool    grounded = IsGrounded();
        Vector3 offset   = grounded ? groundedOffset : airOffset;
        float   smooth   = grounded ? groundedPosSmooth : airPosSmooth;

        Vector3 target = rootPos + offset;

        if (!_pivotInitialized)
        {
            pivot.position  = target;
            _vel            = Vector3.zero;
            _pivotInitialized = true;
            return;
        }

        // ✅ Çok uzaksa direkt snap — ölüm/respawn sonrası geç kalmayı engeller
        if (Vector3.Distance(pivot.position, target) > snapDistance)
        {
            pivot.position = target;
            _vel           = Vector3.zero;
            return;
        }

        // ✅ Tek bir SmoothDamp — clamp yok, kamera topu kaçırmaz
        pivot.position = Vector3.SmoothDamp(pivot.position, target, ref _vel, smooth);
    }
}