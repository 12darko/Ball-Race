using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Obstacles
{
    public class BouncePadObstacle : Obstacle
    {
 [Header("BouncePad — Ayarlar")]
    [SerializeField] private float bounceMultiplier = 1f;
    [SerializeField] private bool resetVelocityBeforeBounce = true;
 
    [Header("BouncePad — Animasyon")]
    [SerializeField] private Transform padMesh;
    [SerializeField, Range(0f, 1f)] private float squishAmount = 0.4f;
    [SerializeField] private float squishRecoverySpeed = 8f;
 
    [Header("Ses / Efekt")]
    [SerializeField] private ParticleSystem bounceParticle;
    [SerializeField] private AudioSource    bounceSound;
 
    [Networked] private float NetworkedSquish { get; set; }
 
    private Vector3 _baseScale;
    private float   _cooldown;
 
    public override void Spawned()
    {
        if (padMesh != null)
            _baseScale = padMesh.localScale;
    }
 
    public override void FixedUpdateNetwork()
    {
        if (_cooldown > 0f)
            _cooldown -= Runner.DeltaTime;
 
        if (Object.HasStateAuthority && NetworkedSquish > 0f)
        {
            NetworkedSquish -= squishRecoverySpeed * Runner.DeltaTime;
            if (NetworkedSquish < 0f) NetworkedSquish = 0f;
        }
    }
 
    public override void Render()
    {
        if (padMesh == null) return;
 
        float s = NetworkedSquish * squishAmount;
        padMesh.localScale = new Vector3(
            _baseScale.x * (1f + s * 0.3f),
            _baseScale.y * (1f - s),
            _baseScale.z * (1f + s * 0.3f)
        );
    }
 
    private void OnCollisionEnter(Collision collision)
    {
        if (_cooldown > 0f) return;
 
        Rigidbody ballRb = collision.rigidbody;
        if (ballRb == null) return;
 
        // ✅ GetComponentInParent — NetworkObject root'ta olmayabilir
        NetworkObject ballNo = collision.gameObject.GetComponentInParent<NetworkObject>();
        if (ballNo == null || !ballNo.HasStateAuthority) return;
 
        // ✅ Normal kontrolü yok — Scale Y:0.01 yüzünden alt/üst güvenilmez
        // Inspector'da Scale Y'yi en az 0.2 yap!
 
        if (resetVelocityBeforeBounce)
        {
            ballRb.linearVelocity  = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
 
        float force = obstacleForce * bounceMultiplier;
        ballRb.AddForce(Vector3.up * force, ForceMode.Impulse);
 
        Debug.Log($"[BouncePad] Force uygulandı: {force}");
 
        _cooldown = 0.3f;
        RequestSquishRpc();
    }
 
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RequestSquishRpc()
    {
        NetworkedSquish = 1f;
        PlayEffectsRpc();
    }
 
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void PlayEffectsRpc()
    {
        if (bounceParticle != null) bounceParticle.Play();
        if (bounceSound    != null) bounceSound.Play();
    }
    }
}