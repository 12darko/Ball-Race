using System;
using _Main.Scripts.Multiplayer.Player.NetworkInput;
using _Main.Scripts.Multiplayer.UI.Skill_System;
using Fusion;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer.SkillSystem
{
    public class NetworkPlayerSkillPickUp : NetworkBehaviour
    {
        [SerializeField, Networked] private NetworkBool isInPickUpArea { get; set; }
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }

        //Bu kısımda ilk olarak area içine girerse player F tuşu yardımı ile aluyoruz playera  
        [SerializeField] private SkillData pickedSkillData;
        [SerializeField] private SkillList skillList;
        

        public SkillData PickedSkillData => pickedSkillData;

        private UnityAction _onSkillData;

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkPlayerInput playerInput))
            {
                // compute pressed/released state
                var pressed = playerInput.NetworkAllButtons.GetPressed(ButtonsPrevious);
                var released = playerInput.NetworkAllButtons.GetReleased(ButtonsPrevious);

                // store latest input as 'previous' state we had
                ButtonsPrevious = playerInput.NetworkAllButtons;

                if (pressed.IsSet(NetworkPlayerInputEnums.PickUp))
                {
                    if (isInPickUpArea)
                    {
                        _onSkillData?.Invoke();
                    }
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out SkillPickUpMain pickUp))
            {
                isInPickUpArea = true;

                _onSkillData = () =>
                {
                    pickedSkillData = pickUp.PickUpSkillData;
                    if (skillList.CreatedSkillItem != null)
                    {
                        skillList.DeleteSkillForContent();
                        skillList.CreateSkillForContent(pickedSkillData);
                    }
                    else
                    {
                        skillList.CreateSkillForContent(pickedSkillData);
                    }
                };
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isInPickUpArea = false;
        }
    }
}