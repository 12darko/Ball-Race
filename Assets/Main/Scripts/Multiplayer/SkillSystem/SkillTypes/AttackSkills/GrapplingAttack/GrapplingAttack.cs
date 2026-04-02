// ==================== GrapplingAttack.cs ====================
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.AttackSkills
{
    public class GrapplingAttack : Attack
    {
        [SerializeField] private float pullStrength;
        [SerializeField] private float pullRange;
        [SerializeField] private Transform pulledHoldPosition;

        private CancellationTokenSource _cts;

        protected override void UseSkill()
        {
            if (!IsTimerRunning() && !IsTimerExpired()) return;

            if (IsTimerExpired())
            {
                SkillCoolDownTimer = TickTimer.None;
                Runner.Despawn(Object);
                return;
            }

            transform.position = SkillUsingPlayer.networkPlayersManager.LocalRigidBody.position;

            if (Runner.LagCompensation.Raycast(
                    PlayerInput.NetworkCameraPosition,
                    PlayerInput.NetworkAimForward,
                    pullRange,
                    SkillUsingPlayer.Object.InputAuthority,
                    out var hit) && hit.Hitbox != null)
            {
                var triggeredItem = hit.Hitbox.GetComponentInParent<NetworkObject>();
                if (triggeredItem == null) return;

                bool isOther = triggeredItem.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
                if (!isOther) return;

                var rb = hit.Hitbox.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = (pulledHoldPosition.position - hit.Hitbox.transform.position)
                                        * pullStrength * Runner.DeltaTime;
                }
            }
        }

        protected internal override void StopUseSkill() { }

        protected override async void StartSpawnedValues()
        {
            _cts = new CancellationTokenSource();
            try { await Task.Delay(200, _cts.Token); }
            catch { return; }

            if (Runner == null || skillData == null) return;
            SkillCoolDownTimer = TickTimer.CreateFromSeconds(Runner, skillData.SkillDuration);
        }

        public override void Despawned(NetworkRunner runner, bool hasState) => _cts?.Cancel();
    }
}