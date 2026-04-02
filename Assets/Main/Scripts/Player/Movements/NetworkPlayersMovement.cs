using _Main.Scripts.Multiplayer.Player.NetworkInput;
using Fusion;
using UnityEngine;

namespace Player
{
    public class NetworkPlayersMovement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkPlayers networkPlayers;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform ballVisuals;
        [SerializeField] private NetworkPlayersGroundChecker groundChecker;

        [Header("Movement Settings")]
        [SerializeField] private float airAcceleration = 30f;
        [SerializeField] private float stopForce = 25f;// BaseValue 50

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 60f;
        [SerializeField] private float rotationSmoothing = 15f;

        private float _currentVisualRotSpeed;
        private Vector3 _lastRotationAxis;
        private Vector3 _smoothedRotAxis;

        [Header("Networked States")]
        [SerializeField, Networked] public NetworkBool PlayerIsFreeze { get; set; }
        [SerializeField, Networked] public NetworkBool ActiveGrappleMove { get; set; }
        [SerializeField, Networked] public Vector3 ForceDirection { get; set; }

        [Networked] private float NetRotSpeed { get; set; }
        [Networked] private Vector3 NetRotAxis { get; set; }

        private bool IsGrounded => groundChecker != null && groundChecker.IsGrounded();

        public override void FixedUpdateNetwork()
        {
            // ✅ Hem StateAuthority hem InputAuthority simüle etmeli (client-side prediction)
            if (!Object.HasStateAuthority && !Object.HasInputAuthority) return;

            if (PlayerIsFreeze)
            {
                if (Object.HasStateAuthority)
                {
                    rb.linearVelocity = Vector3.zero;
                    WriteRotationNetworkData(Vector3.zero);
                }
                return;
            }

            if (ActiveGrappleMove)
            {
                WriteRotationNetworkData(GetHorizontalVelocity());
                return;
            }

            if (GetInput(out NetworkPlayerInput input))
                Move(input);
            else
                Stop();

            WriteRotationNetworkData(GetHorizontalVelocity());
        }

        public override void Render()
        {
            RotateBallVisuals_Render();
        }

        private Vector3 GetHorizontalVelocity()
        {
            Vector3 v = rb.linearVelocity;
            return new Vector3(v.x, 0, v.z);
        }

        private void WriteRotationNetworkData(Vector3 horizontalVel)
        {
            // ✅ Sadece StateAuthority network'e yazar
            if (!Object.HasStateAuthority) return;

            NetRotSpeed = horizontalVel.magnitude * rotationSpeed;
            if (horizontalVel.sqrMagnitude > 0.0001f)
                NetRotAxis = Vector3.Cross(Vector3.up, horizontalVel.normalized);
        }

        private void Move(NetworkPlayerInput input)
        {
            Vector3 inputDir = new Vector3(
                input.NetworkCameraLookPosition.x, 0,
                input.NetworkCameraLookPosition.z).normalized;

            if (inputDir.magnitude < 0.1f)
            {
                Stop();
                return;
            }

            ForceDirection = inputDir;

            var stats = networkPlayers.networkPlayersManager.NetworkPlayerStats;
            var maxSpeed = stats.NetworkPlayerMaxSpeed;
            float currentAccel = IsGrounded ? stats.NetworkPlayerSpeed : airAcceleration;

            Vector3 currentVel = rb.linearVelocity;
            Vector3 horizontalVel = new Vector3(currentVel.x, 0, currentVel.z);
            Vector3 targetVel = inputDir * maxSpeed;

            Vector3 velDiff = targetVel - horizontalVel;
            Vector3 force = velDiff * currentAccel;

            // ✅ Force clamp — aşırı ivme önlenir
            if (force.magnitude > currentAccel * 2f)
                force = force.normalized * currentAccel * 2f;

            horizontalVel += force * Runner.DeltaTime;

            // ✅ Max speed clamp
            if (horizontalVel.magnitude > maxSpeed)
                horizontalVel = horizontalVel.normalized * maxSpeed;

            rb.linearVelocity = new Vector3(horizontalVel.x, currentVel.y, horizontalVel.z);
        }

        private void Stop()
        {
            Vector3 vel = rb.linearVelocity;
            Vector3 horizontal = new Vector3(vel.x, 0, vel.z);

            if (horizontal.sqrMagnitude < 0.05f)
            {
                rb.linearVelocity = new Vector3(0, vel.y, 0);
                ForceDirection = Vector3.zero;
                return;
            }

            float stopRate = IsGrounded ? stopForce : 3f;
            horizontal = Vector3.MoveTowards(horizontal, Vector3.zero, stopRate * Runner.DeltaTime);

            ForceDirection = Vector3.zero;
            rb.linearVelocity = new Vector3(horizontal.x, vel.y, horizontal.z);
        }

        private void RotateBallVisuals_Render()
        {
            if (ballVisuals == null) return;

            float dt = Time.deltaTime;

            _currentVisualRotSpeed = Mathf.Lerp(
                _currentVisualRotSpeed, NetRotSpeed, dt * rotationSmoothing);

            if (NetRotAxis.sqrMagnitude > 0.0001f)
                _smoothedRotAxis = Vector3.Slerp(_smoothedRotAxis, NetRotAxis, dt * rotationSmoothing);

            if (_smoothedRotAxis.sqrMagnitude > 0.0001f)
                _lastRotationAxis = _smoothedRotAxis;

            if (_currentVisualRotSpeed > 0.1f && _lastRotationAxis.sqrMagnitude > 0.0001f)
            {
                ballVisuals.Rotate(
                    _lastRotationAxis,
                    _currentVisualRotSpeed * dt,
                    Space.World);
            }
        }
    }
}