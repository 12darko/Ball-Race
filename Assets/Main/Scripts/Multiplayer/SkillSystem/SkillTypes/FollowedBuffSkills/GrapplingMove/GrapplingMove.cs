// ==================== GrapplingMove.cs ====================
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.FollowedBuffs
{
    public class GrapplingMove : FollowedBuff
    {
        [SerializeField] private LayerMask grappleMask;
        [SerializeField] private float grappleStrength = 12f;
        [SerializeField] private float abilityRange = 25f;
        [SerializeField] private LineRenderer grappleLr;

        [Networked] private NetworkBool Grappling { get; set; }
        [Networked] private Vector3 GrappleVector { get; set; }
        [Networked] public Vector3 GrapplePoint { get; set; }

        // FIX: Her tick tetiklenmeyi önle
        private bool _grappleStarted;
        private CancellationTokenSource _cts;

        private void LateUpdate()
        {
            if (Grappling && grappleLr != null && grappleLr.enabled)
                grappleLr.SetPosition(0, transform.position);
        }

        protected override void UseSkill()
        {
            if (!IsTimerRunning() && !IsTimerExpired())
                return;

            if (IsTimerExpired())
            {
                Grappling = false;
                if (grappleLr != null) grappleLr.enabled = false;
                SkillCoolDownTimer = TickTimer.None;
                Runner.Despawn(Object);
                return;
            }

            // FIX: Sadece ilk tick'te başlat; sonrasında force uygulamaya devam et
            transform.position = SkillUsingPlayer.networkPlayersManager.LocalRigidBody.position;

            if (!_grappleStarted)
            {
                _grappleStarted = true;
                TryStartGrapple();
            }

            // Her tick force uygula (grapple aktifse)
            if (Grappling)
            {
                SkillUsingPlayer.networkPlayersManager.LocalRigidBody.AddForce(
                    GrappleVector * grappleStrength * Runner.DeltaTime, ForceMode.Impulse);

                if (grappleLr != null)
                {
                    grappleLr.enabled = true;
                    grappleLr.SetPosition(1, GrapplePoint);
                }
            }
        }

        private void TryStartGrapple()
        {
            if (Runner.GetPhysicsScene().Raycast(
                    PlayerInput.NetworkCameraPosition,
                    PlayerInput.NetworkAimForward,
                    out var hitInfo,
                    abilityRange,
                    grappleMask))
            {
                GrapplePoint = hitInfo.point;
                GrappleVector = Vector3.Normalize(hitInfo.point - transform.position);
                if (GrappleVector.y > 0f)
                    GrappleVector = Vector3.Normalize(GrappleVector + Vector3.up);

                Grappling = true;
            }
            else
            {
                // FIX: Raycast tutmadıysa grapple başlamasın, LineRenderer açık kalmasın
                Grappling = false;
                if (grappleLr != null) grappleLr.enabled = false;
            }
        }

        protected internal override void StopUseSkill()
        {
            Grappling = false;
            _grappleStarted = false;
            if (grappleLr != null) grappleLr.enabled = false;
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
                return;
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