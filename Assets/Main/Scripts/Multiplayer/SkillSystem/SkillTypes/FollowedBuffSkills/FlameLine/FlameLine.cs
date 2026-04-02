// ==================== FlameLine.cs ====================
using System.Threading;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.Player.NetworkInput;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.FollowedBuffs
{
    public class FlameLine : FollowedBuff
    {
        [SerializeField] private float flameYOffset;
        [SerializeField] private float buffPower;

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

            // FIX: InputAuthority karşılaştırması korundu ama null kontrolleri eklendi
            if (SkillUsingPlayer == null) return;
            if (Object.InputAuthority.PlayerId != SkillUsingPlayer.Object.InputAuthority.PlayerId) return;

            var rb = SkillUsingPlayer.networkPlayersManager.LocalRigidBody;
            transform.position = new Vector3(rb.position.x, rb.position.y + flameYOffset, rb.position.z);

            var cameraRotate = SkillUsingPlayer.GetComponentInChildren<NetworkPlayerCameraRotate>();
            if (cameraRotate != null)
                rb.AddForce(cameraRotate.CameraForwardPos * buffPower * Runner.DeltaTime, ForceMode.Impulse);
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