using UnityEngine;
using Fusion;

public class BouncingObject : NetworkBehaviour
{
    [Header("Zıplama Ayarları")]
    [SerializeField] private float bounceHeight = 3f;
    [SerializeField] private float gravity = -20f;
    
    [Header("Smoothness")]
    [Range(0f, 1f)]
    [SerializeField] private float smoothness = 0.9f;
    
    [Header("Animasyon")]
    [SerializeField] private bool enableSquashStretch = true;
    [Range(0f, 0.5f)]
    [SerializeField] private float squashAmount = 0.15f;
    
    [Header("Rotation")]
    [SerializeField] private bool rotateInAir = true;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(50f, 100f, 30f);
    
    [Header("Kontrol")]
    [SerializeField] private bool startOnSpawn = true;
    
    [Tooltip("Stop çağrıldığında smooth durma")]
    [SerializeField] private bool smoothStop = true;
    
    [Tooltip("Smooth stop süresi (saniye)")]
    [SerializeField] private float smoothStopDuration = 1f;
    
    [Networked] private Vector3 Velocity { get; set; }
    [Networked] private float StartY { get; set; }
    [Networked] private NetworkBool IsActive { get; set; }
    [Networked] private NetworkBool IsStopping { get; set; } // YENİ: Durma animasyonu
    [Networked] private TickTimer StopTimer { get; set; } // YENİ: Durma zamanlayıcı
    
    private Vector3 originalScale;
    private float currentVelocity;
    private bool onGround;
    private bool isSpawned = false;
    private float stopProgress = 0f; // Durma ilerlemesi

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public override void Spawned()
    {
        isSpawned = true;
        
        if (Object.HasStateAuthority)
        {
            StartY = transform.position.y;
            IsActive = startOnSpawn;
            IsStopping = false;
            
            if (IsActive)
            {
                float bounceForce = Mathf.Sqrt(2f * Mathf.Abs(gravity) * bounceHeight);
                Velocity = Vector3.up * bounceForce;
            }
            else
            {
                Velocity = Vector3.zero;
            }
        }
        
        currentVelocity = Velocity.y;
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        isSpawned = false;
    }

    public override void FixedUpdateNetwork()
    {
        // Smooth stop durumu
        if (IsStopping)
        {
            if (Object.HasStateAuthority)
            {
                // Timer bittiyse tamamen durdur
                if (StopTimer.ExpiredOrNotRunning(Runner))
                {
                    IsStopping = false;
                    IsActive = false;
                    currentVelocity = 0f;
                    Velocity = Vector3.zero;
                    
                    // Yere indir
                    Vector3 pos = transform.position;
                    pos.y = StartY;
                    transform.position = pos;
                    transform.localScale = originalScale;
                    return;
                }
                
                // Kalan süreyi hesapla
                float remainingTime = StopTimer.RemainingTime(Runner) ?? 0f;
                stopProgress = 1f - (remainingTime / smoothStopDuration);
                
                // Yavaşça azalt
                float slowdownFactor = Mathf.Lerp(1f, 0.3f, stopProgress);
                currentVelocity += gravity * Runner.DeltaTime * slowdownFactor;
                
                Vector3 newPos = transform.position;
                newPos.y += currentVelocity * Runner.DeltaTime;
                
                if (newPos.y <= StartY)
                {
                    newPos.y = StartY;
                    onGround = true;
                    
                    // Zıplama gücü azalarak devam et
                    float bounceForce = Mathf.Sqrt(2f * Mathf.Abs(gravity) * bounceHeight);
                    currentVelocity = bounceForce * slowdownFactor;
                }
                else
                {
                    onGround = false;
                }
                
                transform.position = newPos;
                Velocity = Vector3.up * currentVelocity;
            }
            else
            {
                currentVelocity = Velocity.y;
            }
            return;
        }
        
        // Normal durma (aktif değil)
        if (!IsActive)
        {
            currentVelocity = 0f;
            onGround = true;
            return;
        }
        
        // Normal zıplama
        if (Object.HasStateAuthority)
        {
            currentVelocity += gravity * Runner.DeltaTime;
            
            Vector3 newPos = transform.position;
            newPos.y += currentVelocity * Runner.DeltaTime;
            
            if (newPos.y <= StartY)
            {
                newPos.y = StartY;
                onGround = true;
                
                float bounceForce = Mathf.Sqrt(2f * Mathf.Abs(gravity) * bounceHeight);
                currentVelocity = bounceForce;
            }
            else
            {
                onGround = false;
            }
            
            transform.position = newPos;
            Velocity = Vector3.up * currentVelocity;
        }
        else
        {
            currentVelocity = Velocity.y;
        }
    }

    private void Update()
    {
        if (!isSpawned) return;
        
        // Duruyorsa veya aktif değilse
        if (!IsActive && !IsStopping)
        {
            transform.localScale = originalScale;
            return;
        }
        
        if (rotateInAir && !onGround)
        {
            // Durma animasyonunda rotasyon yavaşlasın
            float rotationMultiplier = IsStopping ? Mathf.Lerp(1f, 0.2f, stopProgress) : 1f;
            transform.Rotate(rotationSpeed * Time.deltaTime * rotationMultiplier);
        }
        
        if (enableSquashStretch)
        {
            if (onGround)
            {
                float squashMultiplier = IsStopping ? Mathf.Lerp(1f, 0.5f, stopProgress) : 1f;
                transform.localScale = new Vector3(
                    originalScale.x * (1f + squashAmount * squashMultiplier),
                    originalScale.y * (1f - squashAmount * squashMultiplier),
                    originalScale.z * (1f + squashAmount * squashMultiplier)
                );
            }
            else
            {
                transform.localScale = Vector3.Lerp(
                    transform.localScale,
                    new Vector3(
                        originalScale.x * (1f - squashAmount * 0.5f),
                        originalScale.y * (1f + squashAmount * 0.5f),
                        originalScale.z * (1f - squashAmount * 0.5f)
                    ),
                    Time.deltaTime * 10f
                );
            }
        }
    }

    public void StartBouncing()
    {
        if (Object.HasStateAuthority)
        {
            IsActive = true;
            IsStopping = false; // Durma iptal
            StopTimer = TickTimer.None;
            
            float bounceForce = Mathf.Sqrt(2f * Mathf.Abs(gravity) * bounceHeight);
            currentVelocity = bounceForce;
            Velocity = Vector3.up * bounceForce;
        }
    }
    
    public void StopBouncing()
    {
        if (Object.HasStateAuthority)
        {
            if (smoothStop)
            {
                // Smooth stop başlat
                IsStopping = true;
                StopTimer = TickTimer.CreateFromSeconds(Runner, smoothStopDuration);
                stopProgress = 0f;
            }
            else
            {
                // Anında durdur (eski davranış)
                IsActive = false;
                IsStopping = false;
                currentVelocity = 0f;
                Velocity = Vector3.zero;
                
                Vector3 pos = transform.position;
                pos.y = StartY;
                transform.position = pos;
                
                transform.localScale = originalScale;
            }
        }
    }
    
    /// <summary>
    /// Anında durdur (smooth olmadan)
    /// </summary>
    public void StopBouncingImmediately()
    {
        if (Object.HasStateAuthority)
        {
            IsActive = false;
            IsStopping = false;
            StopTimer = TickTimer.None;
            currentVelocity = 0f;
            Velocity = Vector3.zero;
            
            Vector3 pos = transform.position;
            pos.y = StartY;
            transform.position = pos;
            
            transform.localScale = originalScale;
        }
    }
    
    public void ToggleBouncing()
    {
        if (!isSpawned) return;
        
        if (IsActive || IsStopping)
        {
            StopBouncing();
        }
        else
        {
            StartBouncing();
        }
    }
    
    public bool IsBouncing()
    {
        if (!isSpawned) return false;
        return IsActive || IsStopping; // Durma animasyonu sırasında da true
    }
    
    public void SetBounceHeight(float newHeight)
    {
        if (Object.HasStateAuthority)
        {
            bounceHeight = newHeight;
            
            if (isSpawned && IsActive)
            {
                float bounceForce = Mathf.Sqrt(2f * Mathf.Abs(gravity) * bounceHeight);
                currentVelocity = bounceForce;
            }
        }
    }
    
    public void ResetPosition()
    {
        if (Object.HasStateAuthority)
        {
            Vector3 pos = transform.position;
            pos.y = StartY;
            transform.position = pos;
            currentVelocity = 0f;
        }
    }
}