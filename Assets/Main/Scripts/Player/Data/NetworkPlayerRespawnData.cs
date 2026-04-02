using Fusion;
using UnityEngine;
using Player; // senin GroundChecker namespace'i Player

namespace Main.Scripts.Player
{
    public class NetworkPlayerRespawnData : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkPlayersGroundChecker groundChecker;

        [Header("Save Rate")]
        [SerializeField] private float updateInterval = 0.2f;

        [Networked] public Vector3 LastSafePosition { get; set; }
        [Networked] public Quaternion LastSafeRotation { get; set; }

        private float _timer;

        public override void Spawned()
        {
            if (groundChecker == null)
                groundChecker = GetComponentInChildren<NetworkPlayersGroundChecker>();

            if (Object != null && (Object.HasStateAuthority || (Runner != null && (Runner.IsServer || Runner.IsSharedModeMasterClient))))
            {
                LastSafePosition = transform.position;
                LastSafeRotation = transform.rotation;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!IsServerOrMaster()) return;
            if (groundChecker == null) return;

            _timer += Runner.DeltaTime;
            if (_timer < updateInterval) return;
            _timer = 0f;

            if (!groundChecker.IsGrounded())
                return;

            LastSafePosition = transform.position;
            LastSafeRotation = transform.rotation;
        }

        private bool IsServerOrMaster()
        {
            if (Runner == null) return false;
            return Runner.IsServer || Runner.IsSharedModeMasterClient;
        }
    }
}