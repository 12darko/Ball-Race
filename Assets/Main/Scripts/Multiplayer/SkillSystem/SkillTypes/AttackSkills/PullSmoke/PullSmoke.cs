// ==================== PullSmoke.cs ====================
using System.Threading;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.Player;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.AttackSkills.PullSmoke
{
    public class PullSmoke : Attack
    {
        [SerializeField] private float pullRadius = 10f;
        [SerializeField] private float pullInfluenceRange;
        [SerializeField] private float pullIntensity;

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

            var explosionPosition = SkillUsingPlayer.transform.position;
            transform.position = SkillUsingPlayer.networkPlayersManager.LocalRigidBody.position;

            // FIX: _hits temizlendi
            _hits.Clear();
            SkillUsingPlayer.Runner.LagCompensation.OverlapSphere(
                explosionPosition, pullRadius,
                SkillUsingPlayer.Object.InputAuthority, _hits);

            foreach (var item in _hits)
            {
                if (item.Hitbox == null) continue;

                var triggeredItem = item.Hitbox.GetComponentInParent<NetworkObject>();
                if (triggeredItem == null) continue;

                bool isOtherPlayer = triggeredItem.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
                if (!isOtherPlayer) continue;

                var otherSkillUseManager = triggeredItem.GetComponentInChildren<NetworkPlayerSkillUseManager>();
                if (otherSkillUseManager != null && otherSkillUseManager.OtherPlayerNotUseSkillOnMyOwn)
                    continue; // FIX: return yerine continue — diğer oyuncuları işlemeye devam et

                float dist = Vector3.Distance(triggeredItem.transform.position, transform.position);
                if (dist <= pullInfluenceRange)
                {
                    var pullForce = (transform.position - triggeredItem.transform.position).normalized
                                   / dist * pullIntensity;
                    var rb = item.Hitbox.GetComponent<Rigidbody>();
                    if (rb != null) rb.AddForce(pullForce, ForceMode.Impulse);
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