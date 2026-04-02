using Fusion;

namespace Player
{
    public class NetworkPlayerKnockbackTracker : NetworkBehaviour
    {
        [Networked] public PlayerRef  LastHitBy      { get; set; }
        [Networked] public TickTimer  HitExpireTimer { get; set; }

        private const float HIT_MEMORY_SECONDS = 5f;

        /// Skill / attack kodundan server-side çağır
        public void RegisterHit(PlayerRef attacker)
        {
            if (!Runner.IsServer) return;
            LastHitBy      = attacker;
            HitExpireTimer = TickTimer.CreateFromSeconds(Runner, HIT_MEMORY_SECONDS);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;
            if (HitExpireTimer.Expired(Runner))
                LastHitBy = PlayerRef.None;
        }
    }
}