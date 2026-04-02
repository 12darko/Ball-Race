using _Main.Scripts.Multiplayer.Multiplayer;
using Fusion;
using Fusion.Addons.Physics;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Player
{
    public class NetworkPlayersManager : MonoBehaviour
    {
        #region References

        [Header("References")]
        [SerializeField] private NetworkPlayerInputManager networkPlayerInputManager;
        [SerializeField] private NetworkPlayersMovement networkPlayersMovement;
        [SerializeField] private NetworkPlayerStats networkPlayerStats;
        [SerializeField] private NetworkPlayerHud networkPlayerHud;
        [SerializeField] private NetworkPlayerSkillUseManager networkPlayerSkillUseManager;
        [SerializeField] private NetworkPlayerLookAt networkPlayerLookAt;
        [SerializeField] private NetworkPlayerCameraTarget networkPlayerCameraTarget;
        [SerializeField] private NetworkPlayerCameraRotate networkPlayerCameraRotate;

        public NetworkPlayerInputManager NetworkPlayerInputManager => networkPlayerInputManager;
        public NetworkPlayersMovement NetworkPlayersMovement => networkPlayersMovement;
        public NetworkPlayerStats NetworkPlayerStats => networkPlayerStats;
        public NetworkPlayerHud NetworkPlayerHud => networkPlayerHud;
        public NetworkPlayerSkillUseManager NetworkPlayerSkillUseManager => networkPlayerSkillUseManager;
        public NetworkPlayerLookAt NetworkPlayerLookAt => networkPlayerLookAt;
        public NetworkPlayerCameraTarget NetworkPlayerCameraTarget => networkPlayerCameraTarget;
        public NetworkPlayerCameraRotate NetworkPlayerCameraRotate => networkPlayerCameraRotate;

        #endregion

        #region Components

        [Header("Components")]
        [SerializeField] private SphereCollider sphereCollider;
        [SerializeField] private Transform ballModel;
        [SerializeField] private Rigidbody localRigidBody;
        [SerializeField] private Camera localCamera;
        [SerializeField] private CinemachineCamera localCinemachineCamera;
        [SerializeField] private Camera stageCamera;

        [Header("Network")]
        [SerializeField] private NetworkRigidbody3D networkRigidbody3D;
        [SerializeField] private Transform interpolationTarget; // ✅ Boş child obje

        public SphereCollider SphereCollider => sphereCollider;
        public Transform BallModel => ballModel;
        public Rigidbody LocalRigidBody => localRigidBody;
        public Camera LocalCamera => localCamera;
        public Camera StageCamera
        {
            get => stageCamera;
            set => stageCamera = value;
        }
        public CinemachineCamera LocalCinemachineCamera => localCinemachineCamera;

        // ✅ Fusion interpolated pozisyon - hat/face/kamera için
        public Vector3 SmoothedBallPosition
        {
            get
            {
                if (interpolationTarget != null)
                    return interpolationTarget.position;
                if (networkRigidbody3D != null && networkRigidbody3D.InterpolationTarget != null)
                    return networkRigidbody3D.InterpolationTarget.position;
                return ballModel != null ? ballModel.position : Vector3.zero;
            }
        }

        #endregion
    }
}