// ==================== SkillPickUpRandom.cs ====================
using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Multiplayer.SkillSystem
{
    public class SkillPickUpRandom : SkillPickUpMain
    {
        [Networked] private TickTimer RandomSkillTimer { get; set; }

        // FIX: [SerializeField, Networked] kaldırıldı.
        [Networked] private NetworkString<_32> RandomSkillHasTime { get; set; }

        [SerializeField] private float timerDuration;
        [SerializeField] private SkillData[] skillDatas;

        public override void Spawned()
        {
            if (!HasStateAuthority) return;
            RandomSkillTimer = TickTimer.CreateFromSeconds(Runner, timerDuration);
            PickUpSkillData = skillDatas[Random.Range(0, skillDatas.Length)];
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            if (RandomSkillTimer.Expired(Runner))
            {
                PickUpSkillData = skillDatas[Random.Range(0, skillDatas.Length)];
                RandomSkillTimer = TickTimer.CreateFromSeconds(Runner, timerDuration);
            }
            else if (RandomSkillTimer.RemainingTime(Runner).HasValue)
            {
                var timeSpan = TimeSpan.FromSeconds(RandomSkillTimer.RemainingTime(Runner).Value);
                RandomSkillHasTime = $"{timeSpan.Minutes:D2} : {timeSpan.Seconds:D2}";
            }
        }
    }
}