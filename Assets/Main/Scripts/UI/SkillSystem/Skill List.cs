using System;
using System.Collections.Generic;
using Fusion;
using Multiplayer.SkillSystem;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.UI.Skill_System
{
    public class SkillList : NetworkBehaviour
    {
        [SerializeField] private Transform skillContent;
        [SerializeField] private SkillItem skillItem;
        [SerializeField] private SkillData skillData;
        [SerializeField, Networked] public SkillItem CreatedSkillItem { get; set; }
        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        public void CreateSkillForContent(SkillData skillData)
        {
            if (Runner.IsServer)
            {
                Runner.Spawn(skillItem, skillContent.transform.position, Quaternion.identity, Object.InputAuthority, onBeforeSpawned:
                    (runner, o) =>
                    {
                        CreatedSkillItem = o.GetComponent<SkillItem>();
                    });
             
            }

            this.skillData = skillData;
        }

        public void DeleteSkillForContent()
        {
            Destroy(CreatedSkillItem.gameObject);
            Runner.Despawn(CreatedSkillItem.Object);
            CreatedSkillItem = null;
        }
        
        public override void Render()
        {
            foreach (var change in  _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(CreatedSkillItem):
                        //Todo Change Sistemi Değiştiği için bu şekil kullanıyoruz
                        //var reader = GetPropertyReader<NetworkString<_32>>(nameof(PlayerName));
                        //var currentAmount = reader.Read(current);
                        SetParent(CreatedSkillItem, skillContent.GetComponent<NetworkObject>());
                        break;
                }
            }
        }
        
      
        private void SetParent(SkillItem createdSkillItem, NetworkObject content) //Trsp client da rpc kullanarak set parent edebilirz ama OnChanged denicez şimdi
        {
            if (createdSkillItem == null)
            {
                return;
            }
 
            createdSkillItem.transform.SetParent(content.transform, false); //Trsp yapınca spawn edilen itemı scrollview kısmındaki bug gideriliyor
            createdSkillItem.transform.position = Vector3.zero;;
           createdSkillItem.SkillData = skillData;
           createdSkillItem.SkillItemIcon.sprite = skillData.SkillIcon; 
      
        }
        
    }
}