using Fusion;
using UnityEngine;

public class NetworkObstacleRotator : NetworkBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Transform target;              
    [SerializeField] private Vector3 axis = Vector3.up;     
    [SerializeField] private float degreesPerSecond = 180f; // HIZ

    [Header("Push Settings")]
    [SerializeField] private float pushForce = 25f;         // ileri itme
    [SerializeField] private float upwardForce = 4f;        // hafif zıplatma
    [SerializeField] private float pushCooldown = 0.15f;    // spam engeli

    private float _nextPushTime;

    public override void Spawned()
    {
        if (target == null)
            target = transform;
    }

    public override void FixedUpdateNetwork()
    {
        // 🔒 sadece server döndürür
        if (!Object.HasStateAuthority) return;

        target.Rotate(axis, degreesPerSecond * Runner.DeltaTime, Space.Self);
    }

    private void OnTriggerStay(Collider other)
    {
        // 🔒 sadece server iter
        if (!Object.HasStateAuthority) return;

        if (Time.time < _nextPushTime) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // itme yönü: fan merkezinden oyuncuya doğru
        Vector3 dir = (rb.position - target.position);
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        dir.Normalize();

        Vector3 force = dir * pushForce + Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.VelocityChange);

        _nextPushTime = Time.time + pushCooldown;
    }
}