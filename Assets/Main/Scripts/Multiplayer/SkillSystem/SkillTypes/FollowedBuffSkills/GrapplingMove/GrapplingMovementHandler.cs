// ==================== GrapplingMovementHandler.cs ====================
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.FollowedBuffs
{
    public class GrapplingMovementHandler : NetworkBehaviour
    {
        // FIX: [SerializeField, Networked] kaldırıldı
        [Networked] private Vector3 VelocityToSet { get; set; }
        [Networked] private float Gravity { get; set; }

        [SerializeField] private float gravityMultiplier = 3.5f;

        public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight, NetworkPlayers skillUsingPlayer)
        {
            skillUsingPlayer.networkPlayersManager.NetworkPlayersMovement.ActiveGrappleMove = true;
            VelocityToSet = CalculateJumpVelocity(
                skillUsingPlayer.transform.position,
                targetPosition,
                trajectoryHeight);
            skillUsingPlayer.networkPlayersManager.LocalRigidBody.linearVelocity = VelocityToSet;
        }

        private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
        {
            Gravity = Physics.gravity.y * gravityMultiplier;
            var displacementY = endPoint.y - startPoint.y;
            var displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            var velocityY = Vector3.up * Mathf.Sqrt(-2f * Gravity * trajectoryHeight);
            var velocityXZ = displacementXZ / (Mathf.Sqrt(-2f * trajectoryHeight / Gravity)
                                               + Mathf.Sqrt(2f * (displacementY - trajectoryHeight) / Gravity));

            return velocityXZ + velocityY;
        }
    }
}