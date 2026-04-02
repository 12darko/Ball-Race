// ==================== SkillCreateForObjectsInPlayer.cs ====================
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem
{
    public class SkillCreateForObjectsInPlayer : NetworkBehaviour
    {
        [SerializeField] private SkillData skillData;

        private void OnTriggerEnter(Collider other)
        {
            // FIX: parent null olabilir
            if (other.transform.parent == null) return;
            if (!other.transform.parent.TryGetComponent(out NetworkObject networkObject)) return;

            if (!Runner.IsServer) return;

            // FIX: Önceki kodda SetSkillData, spawn callback'ten ÖNCE çağrılıyordu
            // (skillObject henüz null). Spawn callback içine taşıdık.
            var capturedSkillData = skillData;
            var capturedNetworkObject = networkObject;

            Runner.Spawn(
                skillData.SkillType.gameObject,
                networkObject.transform.GetChild(0).position,
                Quaternion.identity,
                networkObject.InputAuthority,
                (runner, spawnedObject) =>
                {
                    var skill = spawnedObject.GetComponent<Skill>();
                    if (skill == null) return;

                    // Server'da doğrudan Networked property'leri set edebiliriz.
                    skill.SkillUsingPlayer = capturedNetworkObject.GetComponent<NetworkPlayers>();
                    skill.skillData = capturedSkillData;

                    var playerSkillManager = capturedNetworkObject.GetComponentInChildren<SkillManager>();
                    if (playerSkillManager != null)
                    {
                        playerSkillManager.UsingSkill = skill;
                        playerSkillManager.SkillData = capturedSkillData;
                    }
                });
        }
    }
}