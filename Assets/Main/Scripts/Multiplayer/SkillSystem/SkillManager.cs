// ==================== SkillManager.cs ====================
using Fusion;
using Multiplayer.SkillSystem.SkillTypes;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem
{
    public class SkillManager : NetworkBehaviour
    {
        [SerializeField] private NetworkPlayers players;

        // FIX: [SerializeField] kaldırıldı — Networked property'lerle kullanılamaz.
        [Networked] public Skill UsingSkill { get; set; }

        [SerializeField] private SkillData skillData;

        public SkillData SkillData
        {
            get => skillData;
            set => skillData = value;
        }

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this, out _, out _))
            {
                switch (change)
                {
                    case nameof(UsingSkill):
                        OnChangedUsingSkill(UsingSkill);
                        break;
                }
            }
        }

        private void OnChangedUsingSkill(Skill usingSkill)
        {
            if (usingSkill == null) return;

            switch (usingSkill)
            {
                case Attack attack:
                    // Saldırı skilllerine özel setup buraya
                    break;

                case Buff buff:
                    // Buff setup
                    break;

                case FollowedBuff followedBuff:
                    // FIX: Networked property'leri Render() içinden set etmek
                    // sadece StateAuthority'de (server) yapılmalı.
                    // SkillUsingPlayer zaten spawn callback'te set edildi.
                    // Burada sadece local (non-networked) görsel güncellemeler yapılabilir.
                    if (HasStateAuthority)
                    {
                        // skillData server'da zaten atandı; burada loglama vb. yapılabilir.
                        Debug.Log($"FollowedBuff aktif: {followedBuff.name}");
                    }
                    break;
            }
        }
    }
}