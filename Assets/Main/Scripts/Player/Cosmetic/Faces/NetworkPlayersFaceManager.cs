using Fusion;
using Main.Scripts.Player.Database;
using UnityEngine;

namespace Player.Cosmetic.Faces
{
    public class NetworkPlayersFaceManager : NetworkBehaviour
    {
        [Header("Offset Settings")]
        [SerializeField] private float networkPlayerFaceParentYOffset;
        [SerializeField] private float networkPlayerFaceParentZOffset;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 1500f;
        [SerializeField] private float maxLookUpAngle = 45f;
        [SerializeField] private float maxLookDownAngle = 60f;

        [Header("Face Rotation Offset")]
        [SerializeField] private Vector3 faceLocalRotationOffset = new Vector3(90f, 0f, 0f);

        [Header("References")]
        [SerializeField] private NetworkPlayers networkPlayers;
        [SerializeField] private CosmeticDatabase cosmeticDB;
        [SerializeField] private Transform networkPlayerFaceParent;

        [Networked, OnChangedRender(nameof(OnFaceIndexChanged))]
        public int NetworkedFaceIndex { get; set; }

        private GameObject _localFaceObject;

        private void OnFaceIndexChanged()
        {
            SpawnLocalFace(NetworkedFaceIndex);
        }

        public void SpawnFace(int faceIndex)
        {
            if (!Runner.IsServer) return;
            NetworkedFaceIndex = faceIndex;
        }

        private void SpawnLocalFace(int faceIndex)
        {
            if (_localFaceObject != null)
            {
                Destroy(_localFaceObject);
                _localFaceObject = null;
            }

            var faceList = cosmeticDB.allFaces;
            if (faceIndex < 0 || faceIndex >= faceList.Count) return;

            var selectedItem = faceList[faceIndex];
            if (selectedItem.CustomizeItemPrefab == null)
            {
                Debug.LogWarning($"[FaceManager] CustomizeItemPrefab null! faceIndex={faceIndex}");
                return;
            }

            _localFaceObject = Instantiate(selectedItem.CustomizeItemPrefab, networkPlayerFaceParent);
            _localFaceObject.transform.localPosition = new Vector3(0, networkPlayerFaceParentYOffset, networkPlayerFaceParentZOffset);
            _localFaceObject.transform.localRotation = Quaternion.Euler(faceLocalRotationOffset);
        }

        public override void Spawned()
        {
            if (NetworkedFaceIndex >= 0)
                SpawnLocalFace(NetworkedFaceIndex);
        }

        public override void Render()
        {
            if (_localFaceObject == null) return;
            if (networkPlayerFaceParent == null || networkPlayers == null || networkPlayers.networkPlayersManager == null) return;

            // ✅ SmoothedBallPosition - remote jitter yok
            var ballPos = networkPlayers.networkPlayersManager.SmoothedBallPosition;
            var ballT = networkPlayers.networkPlayersManager.BallModel;

            networkPlayerFaceParent.position = ballPos;

            _localFaceObject.transform.localPosition = new Vector3(0, networkPlayerFaceParentYOffset, networkPlayerFaceParentZOffset);
            _localFaceObject.transform.localRotation = Quaternion.Euler(faceLocalRotationOffset);

            var cameraRotate = networkPlayers.networkPlayersManager.NetworkPlayerCameraRotate;
            Vector3 lookDir = cameraRotate.NetworkedCameraFullForward;
            if (lookDir.sqrMagnitude < 0.001f) lookDir = Vector3.forward;

            Vector3 up = ballT != null ? ballT.up : Vector3.up;

            Quaternion rawTargetRotation = Quaternion.LookRotation(lookDir, up);
            Vector3 euler = rawTargetRotation.eulerAngles;

            float xAngle = euler.x;
            if (xAngle > 180) xAngle -= 360;
            xAngle = Mathf.Clamp(xAngle, -maxLookUpAngle, maxLookDownAngle);

            Quaternion clampedTargetRotation = Quaternion.Euler(xAngle, euler.y, 0f);

            networkPlayerFaceParent.rotation = Quaternion.RotateTowards(
                networkPlayerFaceParent.rotation,
                clampedTargetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void OnDestroy()
        {
            if (_localFaceObject != null)
                Destroy(_localFaceObject);
        }
    }
}