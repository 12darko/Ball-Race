// ==================== SkillData.cs ====================
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.SkillSystem
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "SkillSystem/SkillData", order = 0)]
    public class SkillData : ScriptableObject
    {
        [SerializeField] private string skillName;
        [SerializeField] private float skillDuration;
        [SerializeField] private Skill skillType;
        [SerializeField] private Sprite skillIcon;

        public string SkillName
        {
            get => skillName;
            set => skillName = value;
        }

        public float SkillDuration
        {
            get => skillDuration;
            set => skillDuration = value;
        }

        public Skill SkillType
        {
            get => skillType;
            set => skillType = value;
        }

        public Sprite SkillIcon
        {
            get => skillIcon;
            set => skillIcon = value;
        }
    }
}