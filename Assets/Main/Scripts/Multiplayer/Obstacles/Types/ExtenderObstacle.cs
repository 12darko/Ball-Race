using _Main.Scripts.Multiplayer.Multiplayer.Obstacles;
using Fusion;
using UnityEngine;

/// <summary>
/// Extender — Photon Fusion 2
/// Sürekli yukarı aşağı hareket eden piston engeli.
/// Topa çarptığında obstacleForce ile yukarı fırlatır.
/// </summary>
public class ExtenderObstacle : Obstacle
{
    [Header("Extender References")]
    [Tooltip("Collider child'ını buraya sürükle. Model otomatik takip eder.")]
    [SerializeField] private Transform colliderTransform;
 
    [Header("Extender — Hareket")]
    [SerializeField] private float extendHeight = 3f;
    [SerializeField] private float extendSpeed  = 2f;
 
    [Header("Ses / Efekt")]
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private AudioSource    hitSound;
 
    [Networked] private float        ExtendAmount { get; set; }
    [Networked] private NetworkBool  GoingUp      { get; set; }
 
    private Vector3 _baseLocalPos;
 
    public override void Spawned()
    {
        if (colliderTransform != null)
            _baseLocalPos = colliderTransform.localPosition;
 
        if (Object.HasStateAuthority)
        {
            ExtendAmount = 0f;
            GoingUp      = true;
        }
    }
 
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
 
        float delta = extendSpeed * Runner.DeltaTime;
 
        if (GoingUp)
        {
            ExtendAmount += delta;
            if (ExtendAmount >= 1f) { ExtendAmount = 1f; GoingUp = false; }
        }
        else
        {
            ExtendAmount -= delta;
            if (ExtendAmount <= 0f) { ExtendAmount = 0f; GoingUp = true; }
        }
    }
 
    public override void Render()
    {
        if (colliderTransform == null) return;
        colliderTransform.localPosition = _baseLocalPos + Vector3.up * (ExtendAmount * extendHeight);
    }
 
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.rigidbody;
        if (ballRb == null) return;
 
        // ✅ Topun authority'sini kontrol et
        NetworkObject ballNo = collision.gameObject.GetComponent<NetworkObject>();
        if (ballNo == null || !ballNo.HasStateAuthority) return;
 
        float forceMultiplier = GoingUp ? 1.5f : 1f;
 
        TryPush(ballRb, Vector3.up * obstacleForce * forceMultiplier);
        TryDamage(collision.gameObject);
 
        if (hitParticle != null)
        {
            hitParticle.transform.position = collision.contacts[0].point;
            hitParticle.Play();
        }
        if (hitSound != null)
            hitSound.Play();
    }
}