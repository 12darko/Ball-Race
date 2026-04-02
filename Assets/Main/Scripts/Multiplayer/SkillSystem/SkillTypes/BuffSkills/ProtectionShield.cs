// ==================== ProtectionShield.cs ====================
using System.Threading;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.Player;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.BuffSkills
{
    public class ProtectionShield : Buff
    {
        [SerializeField] private float protectionRadius;

        private CancellationTokenSource _cts;

        protected override void UseSkill()
        {
            if (!IsTimerRunning() && !IsTimerExpired()) return;

            if (IsTimerExpired())
            {
                SkillCoolDownTimer = TickTimer.None;
                // FIX: Despawn öncesi shield kaldır
                if (SkillUsingPlayer != null)
                    SkillUsingPlayer.networkPlayersManager.NetworkPlayerSkillUseManager.OtherPlayerNotUseSkillOnMyOwn = false;
                Runner.Despawn(Object);
                return;
            }

            var explosionPosition = SkillUsingPlayer.transform.position;
            transform.position = SkillUsingPlayer.networkPlayersManager.LocalRigidBody.position;

            // FIX: _hits her kullanımdan önce temizlenmeli
            _hits.Clear();
            SkillUsingPlayer.Runner.LagCompensation.OverlapSphere(
                explosionPosition, protectionRadius,
                SkillUsingPlayer.Object.InputAuthority, _hits);

            foreach (var item in _hits)
            {
                if (item.Hitbox == null) continue;

                var triggeredItem = item.Hitbox.GetComponentInParent<NetworkObject>();
                if (triggeredItem == null) continue;

                if (triggeredItem.InputAuthority.PlayerId == Object.InputAuthority.PlayerId)
                {
                    var myPlayers = triggeredItem.GetComponentInChildren<NetworkPlayerSkillUseManager>();
                    if (myPlayers != null)
                        myPlayers.OtherPlayerNotUseSkillOnMyOwn = true;
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