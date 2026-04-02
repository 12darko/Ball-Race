using System;
using System.Collections;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Obstacles
{
    public class CannonObstacle : Obstacle
    {
        public static event Action OnCannonFired;
        public static event Action OnCannonReady;

        [Header("Cannon — Referanslar")]
        [SerializeField] private Transform activationPoint;

        [Header("Cannon — Davranış")]
        [SerializeField] private float launchDelay           = 1f;
        [SerializeField] private float cooldownTime          = 2f;
        [SerializeField] private float upwardBias            = 0.5f;
        [SerializeField] private float colliderReEnableDelay = 0.5f;

        [Header("Cannon — Suck-in Efekti")]
        [SerializeField] private float suckInDuration              = 0.4f;
        [SerializeField, Range(0f, 1f)] private float suckInScaleTarget = 0.2f;

        [Header("Ses / Efekt")]
        [SerializeField] private ParticleSystem loadParticle;
        [SerializeField] private ParticleSystem launchParticle;
        [SerializeField] private AudioSource    loadSound;
        [SerializeField] private AudioSource    launchSound;

        // ─────────────────────────────────────────────────────────
        // Networked State
        // ─────────────────────────────────────────────────────────

        private enum CannonState : byte { Idle, Busy }

        [Networked] private CannonState State        { get; set; }

        /// <summary>
        /// 0 = normal scale, 1 = tam suck-in.
        /// Render() bunu tüm clientlarda scale'e uygular.
        /// </summary>
        [Networked] private float NetworkedSuckIn { get; set; }

        /// <summary>Hangi topun scale'i değişecek.</summary>
        [Networked] private NetworkId  LoadedBallId { get; set; }
        [Networked] private float       PopOutTimer  { get; set; }
        [Networked] private NetworkBool IsPopping    { get; set; }

        // ─────────────────────────────────────────────────────────
        // Private
        // ─────────────────────────────────────────────────────────

        private Rigidbody           _loadedRb;
        private NetworkRigidbody3D  _loadedNetRb;
        private Transform           _loadedBallTransform;
        private Vector3             _originalBallScale;
        private CannonPressurePlate _pressurePlate;
        private Collider[]          _cannonColliders;
        private bool                _isProcessing;

        // ─────────────────────────────────────────────────────────
        // Fusion Lifecycle
        // ─────────────────────────────────────────────────────────

        public override void Spawned()
        {
            _pressurePlate   = GetComponentInChildren<CannonPressurePlate>();
            _cannonColliders = GetComponentsInChildren<Collider>();

            if (Object.HasStateAuthority)
            {
                State           = CannonState.Idle;
                NetworkedSuckIn = 0f;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;
            if (!IsPopping) return;

            PopOutTimer += Runner.DeltaTime;
            float t = Mathf.Clamp01(PopOutTimer / 0.2f); // popOutDuration sabit 0.2f

            float scale;
            if (t < 0.6f)
                scale = Mathf.Lerp(suckInScaleTarget, popOutOvershoot, t / 0.6f);
            else
                scale = Mathf.Lerp(popOutOvershoot, 1f, (t - 0.6f) / 0.4f);

            // negatif = büyüme
            NetworkedSuckIn = -(scale - 1f);

            if (t >= 1f)
            {
                IsPopping       = false;
                NetworkedSuckIn = 0f;
                PopOutTimer     = 0f;
            }
        }

        /// <summary>
        /// Render: Tüm clientlarda her frame çalışır.
        /// NetworkedSuckIn değerine göre topu küçültür/büyütür.
        /// NetworkRigidbody3D pozisyonu zaten interpolate ediyor — biz sadece scale yapıyoruz.
        /// </summary>
        public override void Render()
        {
            if (LoadedBallId == default) return;

            NetworkObject ballNo = Runner.FindObject(LoadedBallId);
            if (ballNo == null) return;

            float t = NetworkedSuckIn;
            float scaleValue;
            if (t >= 0f)
                // 0→1: normal → küçük (suck-in)
                scaleValue = Mathf.Lerp(1f, suckInScaleTarget, t);
            else
                // 0→-1: normal → büyük (pop-out overshoot)
                scaleValue = Mathf.Lerp(1f, popOutOvershoot, -t);

            ballNo.transform.localScale = Vector3.one * scaleValue;
        }

        // ─────────────────────────────────────────────────────────
        // PressurePlate → topun sahibi clientta tetiklenir
        // ─────────────────────────────────────────────────────────

        public void OnBallEntered(Collider other)
        {
            if (_isProcessing) return;
            if (State == CannonState.Busy) return;

            NetworkObject ballNo = other.GetComponentInParent<NetworkObject>();
            if (ballNo == null || !ballNo.HasStateAuthority) return;

            Rigidbody ballRb = other.GetComponentInParent<Rigidbody>();
            if (ballRb == null) return;

            _loadedRb            = ballRb;
            _loadedNetRb         = _loadedRb.GetComponent<NetworkRigidbody3D>();
            _loadedBallTransform = _loadedRb.transform;
            _originalBallScale   = _loadedBallTransform.localScale;
            _isProcessing        = true;

            Debug.Log("[Cannon] ✅ Top yakalandı!");

            _loadedRb.linearVelocity  = Vector3.zero;
            _loadedRb.angularVelocity = Vector3.zero;

            IgnoreCannonCollisions(_loadedRb, true);

            // Host'a state + ballId bildir
            SetBusyRpc(ballNo.Id);

            // Topun sahibinde fiziksel suck-in başlat
            StartCoroutine(SuckInAndFire());
        }

        // ─────────────────────────────────────────────────────────
        // Suck-in coroutine — SADECE topun sahibinde fizik
        // Scale → NetworkedSuckIn üzerinden tüm clientlara yansır
        // ─────────────────────────────────────────────────────────

        private IEnumerator SuckInAndFire()
        {
            Vector3 startPos = _loadedRb.transform.position;
            Vector3 endPos   = activationPoint != null ? activationPoint.position : transform.position;

            float elapsed = 0f;
            while (elapsed < suckInDuration)
            {
                if (_loadedRb == null) { CleanUp(); yield break; }

                float t = elapsed / suckInDuration;
                // EaseIn curve
                float curve = t * t;

                // ✅ Fiziksel pozisyon — sadece topun sahibinde
                _loadedRb.transform.position = Vector3.Lerp(startPos, endPos, curve);

                // ✅ NetworkedSuckIn → Render() tüm clientlarda scale uygular
                SetSuckInRpc(curve);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Hedefe ulaş
            if (_loadedRb != null)
                _loadedRb.transform.position = endPos;

            // Fusion snapshot güncelle
            if (_loadedNetRb != null)
                _loadedNetRb.Teleport(endPos);

            // Dondur
            if (_loadedRb != null)
                _loadedRb.constraints = RigidbodyConstraints.FreezeAll;

            SetSuckInRpc(1f);
            BroadcastLoadRpc();

            yield return new WaitForSeconds(launchDelay);

            Fire();
        }

        [Header("Cannon — Pop-out Efekti")]
        [Tooltip("Fırlatırken büyüme + overshooot süresi")]
        [SerializeField] private float popOutDuration = 0.2f;

        [Tooltip("Max büyüme oranı (1.3 = %30 büyür sonra normale döner)")]
        [SerializeField] private float popOutOvershoot = 1.3f;

        private void Fire()
        {
            if (_loadedRb == null) { CleanUp(); return; }

            _loadedRb.constraints    = RigidbodyConstraints.None;
            _loadedRb.linearVelocity = Vector3.zero;

            Vector3 flatDir = transform.right.normalized;
            Vector3 fireDir = (flatDir + Vector3.up * upwardBias).normalized;
            _loadedRb.AddForce(fireDir * obstacleForce, ForceMode.Impulse);

            Debug.Log($"[Cannon] 🚀 Ateş! Yön: {fireDir} Kuvvet: {obstacleForce}");

            BroadcastFireRpc();
            OnCannonFired?.Invoke();

            // ✅ Pop-out: FixedUpdateNetwork'te smooth, tüm clientlarda sync
            StartPopOutRpc();
            StartCoroutine(PostFire());
        }



        private IEnumerator PostFire()
        {
            yield return new WaitForSeconds(colliderReEnableDelay);
            if (_loadedRb != null)
                IgnoreCannonCollisions(_loadedRb, false);

            yield return new WaitForSeconds(cooldownTime - colliderReEnableDelay);
            CleanUp();
        }

        private void CleanUp()
        {
            if (_loadedRb != null)
            {
                _loadedRb.constraints = RigidbodyConstraints.None;
                IgnoreCannonCollisions(_loadedRb, false);
            }

            _loadedRb            = null;
            _loadedNetRb         = null;
            _loadedBallTransform = null;
            _isProcessing        = false;

            SetIdleRpc();
            _pressurePlate?.Reset();
            OnCannonReady?.Invoke();
            Debug.Log("[Cannon] Hazır.");
        }

        // ─────────────────────────────────────────────────────────
        // RPC
        // ─────────────────────────────────────────────────────────

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void StartPopOutRpc()
        {
            IsPopping   = true;
            PopOutTimer = 0f;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void SetBusyRpc(NetworkId ballId)
        {
            State        = CannonState.Busy;
            LoadedBallId = ballId;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void SetIdleRpc()
        {
            State           = CannonState.Idle;
            NetworkedSuckIn = 0f;
            LoadedBallId    = default;
        }

        /// <summary>
        /// Suck-in değerini host'a gönder → tüm clientlar Render()'da okur.
        /// </summary>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void SetSuckInRpc(float value) => NetworkedSuckIn = value;

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void BroadcastLoadRpc()
        {
            if (loadParticle != null) loadParticle.Play();
            if (loadSound    != null) loadSound.Play();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void BroadcastFireRpc()
        {
            if (launchParticle != null) launchParticle.Play();
            if (launchSound    != null) launchSound.Play();
        }

        // ─────────────────────────────────────────────────────────
        // Yardımcı
        // ─────────────────────────────────────────────────────────

        private void IgnoreCannonCollisions(Rigidbody ballRb, bool ignore)
        {
            if (ballRb == null || _cannonColliders == null) return;
            Collider[] ballColliders = ballRb.GetComponentsInChildren<Collider>();
            foreach (Collider cannonCol in _cannonColliders)
                foreach (Collider ballCol in ballColliders)
                    Physics.IgnoreCollision(cannonCol, ballCol, ignore);
        }
    }
}