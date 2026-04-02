using Fusion;
using UnityEngine;

namespace Multiplayer.SkillSystem
{
    public class SkillPickUpSpecific : SkillPickUpMain
    {
        public override void Spawned()
        {
            if (!HasStateAuthority) return;

            if (PickUpSkillData == null)
                Debug.LogWarning($"[SkillPickUpSpecific] {gameObject.name}: SkillData yok! SpawnManager set etmedi mi?");
        }
    }
}
 