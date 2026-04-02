// ==================== AuraCircle.cs ====================
using System.Threading;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.Player;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.AttackSkills
{
    public class AuraCircle : Attack
    {
        // FIX: [SerializeField, Networked] kaldırıldı; basit serializeField yeterli
        // (bunlar sadece server'da kullanılıyor, sync gerekmez)
        [SerializeField] private float explosionPower = 10f;
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private float explosionUpForce = 1f;

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

            // FIX: _hits her kullanımdan önce temizlendi
            _hits.Clear();
            SkillUsingPlayer.Runner.LagCompensation.OverlapSphere(
                explosionPosition, explosionRadius,
                SkillUsingPlayer.Object.InputAuthority, _hits);

            foreach (var item in _hits)
            {
                if (item.Hitbox == null) continue;

                var triggeredItem = item.Hitbox.GetComponentInParent<NetworkObject>();
                if (triggeredItem == null) continue;

                bool isOther = triggeredItem.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
                if (!isOther) continue;

                var otherSkillUseManager = triggeredItem.GetComponentInChildren<NetworkPlayerSkillUseManager>();
                if (otherSkillUseManager != null && otherSkillUseManager.OtherPlayerNotUseSkillOnMyOwn)
                    continue; // FIX: return yerine continue

                var rb = item.Hitbox.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddExplosionForce(explosionPower, explosionPosition, explosionRadius, explosionUpForce, ForceMode.Impulse);
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