using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

namespace Player
{
    public class NetworkPlayersGroundChecker : NetworkBehaviour
    {
        [Header("Components And References")]
        [SerializeField] private SphereCollider collider;
        
        [SerializeField] private LayerMask groundLayerMask;

        [SerializeField] private NetworkPlayers networkplayers;
        [SerializeField] private NetworkPlayersMovement networkPlayersMovement;

        [Header("Variables")] 
        [SerializeField] private float onAirDrag;
        [SerializeField] private float groundDrag;
        
        
        public override void FixedUpdateNetwork()
        {
            
            //Kilit koyabilirz ilerde havada hareket kilidi
            if (IsGrounded())
            {
                networkplayers.networkPlayersManager.LocalRigidBody.linearDamping = groundDrag;
                networkplayers.networkPlayersManager.NetworkPlayersMovement.ActiveGrappleMove = false;
            }
            else
            {
           
                networkplayers.networkPlayersManager.LocalRigidBody.linearDamping  = onAirDrag;
                networkplayers.PlayerState = NetworkPlayerInputState.OnAir;
            }
 
        }

        public bool IsGrounded()
        {
            var hit = Runner.GetPhysicsScene().Raycast(
                collider.bounds.center, Vector3.down,
                collider.bounds.extents.y + networkplayers
                    .networkPlayersManager.NetworkPlayerStats.NetworkPlayerGroundCheckDistance, groundLayerMask);
            Color rayColor;
            rayColor = hit ? Color.green : Color.red;
            Debug.DrawRay(collider.bounds.center,
                Vector3.down * (collider.bounds.extents.y + networkplayers
                    .networkPlayersManager.NetworkPlayerStats.NetworkPlayerGroundCheckDistance), rayColor,
                groundLayerMask);
             return hit;
        }
    }
}