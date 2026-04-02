using System;
using Fusion;
using Multiplayer.SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.UI.Skill_System
{
    public class SkillItem : NetworkBehaviour
    {
        [SerializeField] private SkillData skillData;
        [SerializeField] private Image skillItemIcon;


        public SkillData SkillData
        {
            get => skillData;
            set => skillData = value;
        }

        public Image SkillItemIcon => skillItemIcon;

       
    }
}