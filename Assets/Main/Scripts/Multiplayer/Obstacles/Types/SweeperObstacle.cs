using _Main.Scripts.Multiplayer.Multiplayer.Obstacles;
using Fusion;
using UnityEngine;

/// <summary>
/// Sweeper — Photon Fusion 2
/// Spinner döndürme işlemi Rigidbody.MoveRotation() ile yapılır.
/// transform.rotation direkt değiştirilmez → titreme ve geçme sorunu olmaz.
/// </summary>
public class SweeperObstacle : Obstacle
{
    [Header("Spinner References")]
    [SerializeField] private Transform[] spinners;
 
    [Header("Spinner — Hız & Yön")]
    [SerializeField] private float[] spinnerSpeeds;
    [SerializeField] private float[] spinnerDirections;
 
    [Header("Sweeper — Çarpışma")]
    [SerializeField] private bool applyUpwardComponent = true;
    [SerializeField, Range(0f, 1f)] private float upwardBias = 0.3f;
 
    [Header("Ses / Efekt")]
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private AudioSource hitSound;
 
    [Networked, Capacity(4)]
    private NetworkArray<float> NetworkedAngles => default;
 
    private Rigidbody[] _spinnerRbs;
 
    public override void Spawned()
    {
        _spinnerRbs = new Rigidbody[spinners.Length];
        for (int i = 0; i < spinners.Length; i++)
        {
            if (spinners[i] == null) continue;
 
            _spinnerRbs[i] = spinners[i].GetComponent<Rigidbody>();
            if (_spinnerRbs[i] != null)
            {
                _spinnerRbs[i].isKinematic   = true;
                _spinnerRbs[i].interpolation = RigidbodyInterpolation.Interpolate;
            }
 
            if (Object.HasStateAuthority)
                NetworkedAngles.Set(i, spinners[i].eulerAngles.y);
        }
    }
 
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
 
        for (int i = 0; i < spinners.Length; i++)
        {
            if (spinners[i] == null) continue;
 
            float newAngle = NetworkedAngles[i] + GetSpeed(i) * GetDirection(i) * Runner.DeltaTime;
            if (newAngle >= 360f) newAngle -= 360f;
            if (newAngle <   0f) newAngle += 360f;
 
            NetworkedAngles.Set(i, newAngle);
 
            Quaternion targetRot = Quaternion.Euler(0f, newAngle, 0f);
            if (_spinnerRbs[i] != null)
                _spinnerRbs[i].MoveRotation(targetRot);
            else
                spinners[i].rotation = targetRot;
        }
    }
 
    public override void Render()
    {
        for (int i = 0; i < spinners.Length; i++)
        {
            if (spinners[i] == null) continue;
            if (_spinnerRbs[i] != null) continue;
            spinners[i].rotation = Quaternion.Euler(0f, NetworkedAngles[i], 0f);
        }
    }
 
    private void OnCollisionEnter(Collision collision)
    {
        if (_cooldown > 0f) return;
 
        Rigidbody ballRb = collision.rigidbody;
        if (ballRb == null) return;
 
        // ✅ Topun authority'sini kontrol et
        NetworkObject ballNo = collision.gameObject.GetComponent<NetworkObject>();
        if (ballNo == null || !ballNo.HasStateAuthority) return;
 
        // Kendi Spinner Rigidbody'leriyle çarpışma sayılmasın
        foreach (var rb in _spinnerRbs)
            if (rb != null && rb == ballRb) return;
 
        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 radialDir    = (contactPoint - transform.position);
        radialDir.y          = 0f;
        radialDir            = radialDir.normalized;
 
        Vector3 forceDir = applyUpwardComponent
            ? Vector3.Lerp(radialDir, Vector3.up, upwardBias).normalized
            : radialDir;
 
        TryPush(ballRb, forceDir * obstacleForce);
        TryDamage(collision.gameObject);
 
        _cooldown = 0.2f;
        PlayHitEffectsRpc(contactPoint);
    }
 
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void PlayHitEffectsRpc(Vector3 position)
    {
        if (hitParticle != null)
        {
            hitParticle.transform.position = position;
            hitParticle.Play();
        }
        if (hitSound != null)
            hitSound.Play();
    }
 
    // ─────────────────────────────────────────────────────────────
    // Cooldown (Obstacle base'de yok, burada local tutuyoruz)
    // ─────────────────────────────────────────────────────────────
    private float _cooldown;
 
    // ─────────────────────────────────────────────────────────────
    // Yardımcılar
    // ─────────────────────────────────────────────────────────────
 
    private float GetSpeed(int i)
    {
        if (spinnerSpeeds != null && i < spinnerSpeeds.Length && spinnerSpeeds[i] > 0f)
            return spinnerSpeeds[i];
        return obstacleRotateSpeed;
    }
 
    private float GetDirection(int i)
    {
        if (spinnerDirections == null || i >= spinnerDirections.Length) return 1f;
        float d = spinnerDirections[i];
        return d == 0f ? 1f : Mathf.Sign(d);
    }
 
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (spinners == null) return;
        int count = spinners.Length;
        ResizeArray(ref spinnerSpeeds,     count, obstacleRotateSpeed > 0 ? obstacleRotateSpeed : 90f);
        ResizeArray(ref spinnerDirections, count, 1f);
    }
 
    private static void ResizeArray(ref float[] arr, int newSize, float def)
    {
        float[] old = arr ?? new float[0];
        arr = new float[newSize];
        for (int i = 0; i < newSize; i++)
            arr[i] = i < old.Length ? old[i] : def;
    }
#endif
}