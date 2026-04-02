using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize
{
    public class LobbyCosmeticBinder : NetworkBehaviour
    {
        [Header("Socket Names")]
        [SerializeField] private string hatParentName = "Hat Parent";
        [SerializeField] private string faceParentName = "Face Parent";

        [Header("Lobby Offsets (LOCAL)")]
        [SerializeField] private float hatParentYOffset = 0.5f;
        [SerializeField] private float faceParentYOffset = 0f;
        [SerializeField] private float faceParentZOffset = 0.5f;

        [Header("Face Rotation Fix")]
        [SerializeField] private Vector3 faceLocalRotation = new Vector3(90, 0, 0);

        private Transform _hatParent;
        private Transform _faceParent;
        private bool _initialized;
        
        private bool _hatParented;
        private bool _faceParented;

        [Networked] private NetworkObject HatNO { get; set; }
        [Networked] private NetworkObject FaceNO { get; set; }
        [Networked] private float CurrentHatYOffset { get; set; }

        public void SetCosmetics(NetworkObject hat, int hatIndex, NetworkObject face, int faceIndex, float hatLocalYOffset)
        {
            if (!Object.HasStateAuthority) return;

            HatNO = hat;
            FaceNO = face;
            CurrentHatYOffset = hatLocalYOffset;

            Debug.Log($"[LobbyCosmeticBinder] SetCosmetics: Hat={hat?.name}, Face={face?.name}, HatY={hatLocalYOffset}");
        }

        private void TryInitialize()
        {
            if (_initialized) return;

            _hatParent = transform.Find(hatParentName) ?? FindDeepChild(transform, hatParentName);
            _faceParent = transform.Find(faceParentName) ?? FindDeepChild(transform, faceParentName);

            if (_hatParent == null)
                Debug.LogError($"[LobbyCosmeticBinder] '{hatParentName}' bulunamadı! Ball: {name}");
            if (_faceParent == null)
                Debug.LogError($"[LobbyCosmeticBinder] '{faceParentName}' bulunamadı! Ball: {name}");

            _initialized = (_hatParent != null || _faceParent != null);
        }

        public override void Render()
        {
            if (!_initialized) TryInitialize();
            if (!_initialized) return;

            // Socketleri sabit tut
            if (_hatParent != null)
            {
                _hatParent.localPosition = new Vector3(0f, hatParentYOffset, 0f);
                _hatParent.localRotation = Quaternion.identity;
            }

            if (_faceParent != null)
            {
                _faceParent.localPosition = new Vector3(0f, faceParentYOffset, faceParentZOffset);
                _faceParent.localRotation = Quaternion.identity;
            }

            // HAT bind (bir kere)
            if (!_hatParented && HatNO != null && _hatParent != null)
            {
                var t = HatNO.transform;
                t.SetParent(_hatParent, false);
                t.localPosition = new Vector3(0f, CurrentHatYOffset, 0f);
                t.localRotation = Quaternion.identity;
                
                _hatParented = true;
                Debug.Log($"[LobbyCosmeticBinder] ✅ Hat parented to {_hatParent.name}");
            }

            // FACE bind (bir kere)
            if (!_faceParented && FaceNO != null && _faceParent != null)
            {
                var t = FaceNO.transform;
                t.SetParent(_faceParent, false);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.Euler(faceLocalRotation);
                
                _faceParented = true;
                Debug.Log($"[LobbyCosmeticBinder] ✅ Face parented to {_faceParent.name}");
            }
        }

        private Transform FindDeepChild(Transform parent, string name)
        {
            if (parent == null) return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i);
                if (c.name == name) return c;

                var r = FindDeepChild(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}