using _Main.Scripts.Multiplayer.Multiplayer.Obstacles.Interfaces;
using Fusion;
using UnityEngine;

/// <summary>
/// Tüm obstacle'ların base class'ı.
/// ObstacleManager tarafından moda göre canPush ve canDamage set edilir.
/// </summary>
public abstract class Obstacle : NetworkBehaviour
{
    [Header("Base — Kuvvet")]
    [SerializeField] public float obstacleForce;
    [SerializeField] public float obstacleRotateSpeed;

    [Header("Base — Mod Davranışı")]
    [Tooltip("Otomatik set edilir. Obstacle player'ı itebilir mi?")]
    public bool canPushPlayer = true;

    [Tooltip("Otomatik set edilir. Obstacle player'a hasar verebilir mi?")]
    public bool canDamagePlayer = false;

    [Tooltip("Her çarpışmada verilecek hasar miktarı.")]
    [SerializeField] public float damageAmount = 10f;

    // ─────────────────────────────────────────────────────────────
    // Yardımcı — alt classlar kullanır
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Rigidbody'ye kuvvet uygula (canPush kontrolü dahil).
    /// </summary>
    protected void TryPush(Rigidbody rb, Vector3 force, ForceMode mode = ForceMode.Impulse)
    {
        if (!canPushPlayer) return;
        if (rb == null) return;
        rb.AddForce(force, mode);
    }

    /// <summary>
    /// Player'a hasar ver (canDamage kontrolü dahil).
    /// IObstacleDamageable interface'ini uygulayan objelere etki eder.
    /// </summary>
    protected void TryDamage(GameObject target)
    {
        if (!canDamagePlayer) return;
        if (target == null) return;

        if (target.TryGetComponent<IObstacleDamageable>(out var damageable))
            damageable.TakeDamage(damageAmount);
    }
}

