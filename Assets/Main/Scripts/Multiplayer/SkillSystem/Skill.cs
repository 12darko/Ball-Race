// ==================== Skill.cs ====================
using System.Collections.Generic;
using _Main.Scripts.Multiplayer.Player.NetworkInput;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem
{
    public abstract class Skill : NetworkBehaviour
    {
        [Header("Networked Variables")]
        // FIX: [SerializeField] ile [Networked] birlikte kullanılamaz.
        // Networked property'ler Fusion'ın kendi buffer'ını kullanır.
        [Networked] public NetworkPlayers SkillUsingPlayer { get; set; }
        [Networked] protected internal TickTimer SkillCoolDownTimer { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }

        [Header("Scriptable Data")]
        [SerializeField] protected internal SkillData skillData;

        [Header("Normal Data")]
        // FIX: Her skill kendi listesini temizlemeden kullanıyor; burada başlatıyoruz.
        protected List<LagCompensatedHit> _hits = new List<LagCompensatedHit>();

        public NetworkPlayerInput PlayerInput { get; set; }

        // FIX: FindObjectsByType her spawn'da çok pahalı. 
        // Gerekirse override'a bırakıldı; base'den kaldırıldı.
        public override void Spawned()
        {
            StartSpawnedValues();
        }

        public override void FixedUpdateNetwork()
        {
            if (SkillUsingPlayer == null)
                return;

            UseSkill();
        }

        protected abstract void UseSkill();
        protected internal abstract void StopUseSkill();
        protected abstract void StartSpawnedValues();

        // FIX: TickTimer.None durumunu güvenli kontrol eden yardımcı metod.
        // Timer hiç başlamadıysa Expired=false ama RemainingTime.HasValue=false olur.
        protected bool IsTimerRunning()
        {
            return SkillCoolDownTimer.RemainingTime(Runner).HasValue;
        }

        protected bool IsTimerExpired()
        {
            return SkillCoolDownTimer.Expired(Runner);
        }
    }
}