using _Main.Scripts.Multiplayer.Player.NetworkInput;
using _Main.Scripts.Multiplayer.UI.Skill_System;
using Fusion;
using Multiplayer.SkillSystem;
using Multiplayer.SkillSystem.SkillTypes.FollowedBuffs;
using UnityEngine;

namespace Player
{
    public class NetworkPlayerSkillUseManager : NetworkBehaviour
    {
        [Networked] public NetworkButtons ButtonsPrevious { get; set; }
        [SerializeField, Networked] private NetworkObject playerObject { get; set; }

        [SerializeField, Networked] private Skill skillObject { get; set; }
        [SerializeField, Networked] public NetworkBool OtherPlayerNotUseSkillOnMyOwn { get; set; } = false;

        
        
        [SerializeField] private SkillList skillList;

        public override void FixedUpdateNetwork()
        {
            if (GetInput<NetworkPlayerInput>(out NetworkPlayerInput playerInput))
            {
                // compute pressed/released state
                var pressed = playerInput.NetworkAllButtons.GetPressed(ButtonsPrevious);
                var released = playerInput.NetworkAllButtons.GetReleased(ButtonsPrevious);

                // store latest input as 'previous' state we had
                ButtonsPrevious = playerInput.NetworkAllButtons;

                if (pressed.IsSet(NetworkPlayerInputEnums.UseSkill))
                {
                    if (skillList.CreatedSkillItem != null)
                    {
                        if (Runner.IsServer)
                        {
                            Runner.Spawn(
                                skillList.CreatedSkillItem.GetComponent<SkillItem>().SkillData.SkillType.gameObject,
                                transform.position,
                                Quaternion.identity,
                                Object.InputAuthority,
                                (runner, o) => { skillObject = o.GetComponent<Skill>(); });
                        }

                        SetSkillData(skillObject, skillList, playerInput);
                    }
                }

                if (released.IsSet(NetworkPlayerInputEnums.UseSkill))
                {
                    if (skillObject == null)
                        return;
                    skillObject.StopUseSkill();
                }
            }
        }

        private void SetSkillData(Skill skillObject, SkillList skillList, NetworkPlayerInput playerInput)
        {
            if (skillObject == null)
                return;
            skillObject.transform.position = playerObject.GetComponentInChildren<NetworkPlayers>().transform.position;
            skillObject.SkillUsingPlayer = playerObject.GetComponentInChildren<NetworkPlayers>();
            skillObject.PlayerInput = playerInput;
            skillList.DeleteSkillForContent();
        }
    }
}