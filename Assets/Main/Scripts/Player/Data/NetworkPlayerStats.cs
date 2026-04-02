using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer
{
    [CreateAssetMenu(fileName = "NetworkPlayerStats", menuName = "Network Player/Stats", order = 0)]
    public class NetworkPlayerStats : ScriptableObject
    {
        [Header("Move")]
        [SerializeField] private float networkPlayerSpeed = 20f;
        [SerializeField] private float networkPlayerMaxSpeed = 27f;
        
        [Header("Jump")]
        [SerializeField] private float networkPlayerJumpSpeed = 12f;
        [SerializeField] private float networkPlayerGroundCheckDistance = 0.6f;
        [SerializeField] private int networkPlayerMaxJumpCount = 2;
        
        [Header("Camera")]
        [SerializeField] private float networkPlayerCameraRotationSpeed = 3f;
        
        public float NetworkPlayerSpeed => networkPlayerSpeed;
        public float NetworkPlayerMaxSpeed => networkPlayerMaxSpeed;
        public float NetworkPlayerJumpSpeed => networkPlayerJumpSpeed;
        public float NetworkPlayerGroundCheckDistance => networkPlayerGroundCheckDistance;
        public int NetworkPlayerMaxJumpCount => networkPlayerMaxJumpCount;
        public float NetworkPlayerCameraRotationSpeed => networkPlayerCameraRotationSpeed;
    }
}