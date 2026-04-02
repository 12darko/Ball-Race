using Fusion;
using Main.Scripts.Player.Database;
using UnityEngine;

namespace Player.Cosmetic
{
    public class NetworkPlayersHatManager : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float networkPlayerHatParentYOffset;
        [SerializeField] private float rotationSpeed = 1500f;

        [Range(0f, 1f)]
        [SerializeField] private float tiltFactor = 0.3f;

        [Header("References")]
        [SerializeField] private NetworkPlayers networkPlayers;
        [SerializeField] private CosmeticDatabase cosmeticDB;
        [SerializeField] private Transform networkPlayerHatParent;

        [Networked, OnChangedRender(nameof(OnHatIndexChanged))]
        public int NetworkedHatIndex { get; set; }

        [Networked] private float currentHatYOffset { get; set; }

        private GameObject _localHatObject;

        private void OnHatIndexChanged()
        {
            SpawnLocalHat(NetworkedHatIndex);
        }

        public void SpawnHat(int hatIndex)
        {
            if (!Runner.IsServer) return;

            var hatList = cosmeticDB.allHats;
            if (hatIndex < 0 || hatIndex >= hatList.Count) return;

            var selectedItem = hatList[hatIndex];
            currentHatYOffset = selectedItem.CustomizeInGameSpawnYOffset;
            NetworkedHatIndex = hatIndex;
        }

        private void SpawnLocalHat(int hatIndex)
        {
            if (_localHatObject != null)
            {
                Destroy(_localHatObject);
                _localHatObject = null;
            }

            var hatList = cosmeticDB.allHats;
            if (hatIndex < 0 || hatIndex >= hatList.Count) return;

            var selectedItem = hatList[hatIndex];
            if (selectedItem.CustomizeItemPrefab == null)
            {
                Debug.LogWarning($"[HatManager] CustomizeItemPrefab null! hatIndex={hatIndex}");
                return;
            }

            _localHatObject = Instantiate(selectedItem.CustomizeItemPrefab, networkPlayerHatParent);
            _localHatObject.transform.localPosition = new Vector3(0, currentHatYOffset, 0);
            _localHatObject.transform.localRotation = Quaternion.identity;
        }

        public override void Spawned()
        {
            if (NetworkedHatIndex >= 0)
                SpawnLocalHat(NetworkedHatIndex);
        }

        public override void Render()
        {
            if (_localHatObject == null) return;

            // ✅ SmoothedBallPosition - remote jitter yok
            var ballPos = networkPlayers.networkPlayersManager.SmoothedBallPosition;

            _localHatObject.transform.localPosition = new Vector3(0, currentHatYOffset, 0);
            _localHatObject.transform.localRotation = Quaternion.identity;

            networkPlayerHatParent.position = new Vector3(
                ballPos.x,
                ballPos.y + networkPlayerHatParentYOffset,
                ballPos.z);

            Vector3 targetDirection = Vector3.forward;
            var camScript = networkPlayers.networkPlayersManager.NetworkPlayerCameraRotate;

            if (camScript != null)
            {
                if (Object.HasInputAuthority)
                    targetDirection = camScript.LookAt.forward;
                else if (camScript.CameraForwardPos.sqrMagnitude > 0.001f)
                    targetDirection = camScript.CameraForwardPos;

                targetDirection.y *= tiltFactor;
                targetDirection.Normalize();

                if (targetDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    networkPlayerHatParent.rotation = Quaternion.RotateTowards(
                        networkPlayerHatParent.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
        }

        private void OnDestroy()
        {
            if (_localHatObject != null)
                Destroy(_localHatObject);
        }
    }
}