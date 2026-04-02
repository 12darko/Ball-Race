using Fusion;
using UnityEngine;

namespace Multiplayer.SkillSystem
{
    public class SkillPickUpMain : NetworkBehaviour
    {
        [SerializeField] private SkillData _pickUpSkillData;

        public SkillData PickUpSkillData
        {
            get => _pickUpSkillData;
            set => _pickUpSkillData = value;
        }
    }
}
 