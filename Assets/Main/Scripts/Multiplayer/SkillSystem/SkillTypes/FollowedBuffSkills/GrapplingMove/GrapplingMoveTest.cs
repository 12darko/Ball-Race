// ==================== GrapplingMoveTest.cs ====================
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.FollowedBuffs
{
    public class GrapplingMoveTest : FollowedBuff
    {
        [Header("References")]
        [SerializeField] private GrapplingExecute grapplingExecute;
        [SerializeField] private GrapplingStop grapplingStop;
        [SerializeField] private LayerMask grappleLayerMask;

        [Header("Grappling")]
        [SerializeField] private float maxGrappleDistance = 200f;
        [SerializeField] private LineRenderer grappleLr;
        [SerializeField] private float overShootYAxis;
        [SerializeField] private float delayGrappleTime;

        // FIX: [SerializeField] kaldırıldı
        [Networked] public NetworkBool Grappling { get; set; }
        [Networked] public Vector3 GrapplePoint { get; set; }

        public LineRenderer GrappleLr => grappleLr;
        public float OverShootYAxis => overShootYAxis;
        public float DelayGrappleTime => delayGrappleTime;

        // FIX: Grapple her tick tetiklenmesin diye flag
        private bool _grappleStarted;

        // FIX: async void için cancellation token
        private CancellationTokenSource _cts;

        protected override void UseSkill()
        {
            if (!IsTimerRunning() && !IsTimerExpired())
                return; // Timer henüz başlamadı

            if (IsTimerExpired())
            {
                if (Grappling)
                {
                    StartCoroutine(grapplingStop.StopGrapple(SkillUsingPlayer, grappleLr, this, 0f));
                    Grappling = false;
                    _grappleStarted = false;
                }
                SkillCoolDownTimer = TickTimer.None;
                Runner.Despawn(Object);
                return;
            }

            // FIX: Sadece bir kez başlat
            if (!_grappleStarted)
            {
                _grappleStarted = true;
                StartGrapple();
            }

            // LineRenderer her frame güncelle
            if (Grappling && grappleLr.enabled)
                grappleLr.SetPosition(0, transform.position);
        }

        private void LateUpdate()
        {
            if (Grappling && grappleLr != null && grappleLr.enabled)
                grappleLr.SetPosition(0, transform.position);
        }

        private void StartGrapple()
        {
            Grappling = true;
            SkillUsingPlayer.networkPlayersManager.NetworkPlayersMovement.PlayerIsFreeze = true;

            transform.position = SkillUsingPlayer.networkPlayersManager.LocalRigidBody.position;

            if (Runner.GetPhysicsScene().Raycast(
                    PlayerInput.NetworkCameraPosition,
                    PlayerInput.NetworkAimForward,
                    out var hit,
                    maxGrappleDistance,
                    grappleLayerMask))
            {
                GrapplePoint = hit.point;
                grappleLr.enabled = true;
                grappleLr.SetPosition(1, GrapplePoint);
                StartCoroutine(grapplingExecute.ExecuteGrapple(SkillUsingPlayer, this, delayGrappleTime));
            }
            else
            {
                GrapplePoint = PlayerInput.NetworkCameraPosition + PlayerInput.NetworkAimForward * maxGrappleDistance;
                grappleLr.enabled = true;
                grappleLr.SetPosition(1, GrapplePoint);
                StartCoroutine(grapplingStop.StopGrapple(SkillUsingPlayer, grappleLr, this, delayGrappleTime));
            }
        }

        protected internal override void StopUseSkill()
        {
            if (Grappling)
            {
                StartCoroutine(grapplingStop.StopGrapple(SkillUsingPlayer, grappleLr, this, 0f));
                Grappling = false;
                _grappleStarted = false;
            }
        }

        protected override async void StartSpawnedValues()
        {
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(200, _cts.Token);
            }
            catch
            {
                return; // İptal edildi veya despawn oldu
            }

            if (Runner == null || skillData == null) return;
            SkillCoolDownTimer = TickTimer.CreateFromSeconds(Runner, skillData.SkillDuration);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            _cts?.Cancel();
        }
    }
}