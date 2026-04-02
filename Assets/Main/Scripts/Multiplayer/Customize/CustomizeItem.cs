using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Multiplayer.Multiplayer.Customize
{
    [CreateAssetMenu(fileName = "CustomizeItem", menuName = "Customize/CustomizeItem", order = 0)]
    public class CustomizeItem : ScriptableObject
    {
        [SerializeField] private CustomizeType customizeType;
        [SerializeField] private Sprite customizeItemImage;
        [SerializeField] private GameObject customizeItemModel;
        [SerializeField] private NetworkPrefabRef customizeItemPrefabRef;
        [SerializeField] private NetworkPrefabRef customizeItemPrefabRefLobby;

        [SerializeField] private GameObject customizeItemPrefab;
        [SerializeField] private GameObject customizeItemPrefabLobby;
        [SerializeField] private float customizeItemPrice;
        [SerializeField] private bool customizeItemIsLocked;
        [SerializeField] private Vector3 customizeItemSpawnPosition;
        [SerializeField] private Vector3 customizeItemSpawnRotation;
        [SerializeField] private float customizeItemScale;
        // --- YENİ EKLENEN KISIM ---
        [SerializeField]  [Tooltip("Bu şapkanın kafada ne kadar yukarıda duracağını belirler.")]
        private float customizeInGameSpawnYOffset = 0.35f;
        
        
        public CustomizeType CustomizeType
        {
            get => customizeType;
            set => customizeType = value;
        }

        public Sprite CustomizeItemImage
        {
            get => customizeItemImage;
            set => customizeItemImage = value;
        }

        public GameObject CustomizeItemModel
        {
            get => customizeItemModel;
            set => customizeItemModel = value;
        }

        public NetworkPrefabRef CustomizeItemPrefabRef
        {
            get => customizeItemPrefabRef;
            set => customizeItemPrefabRef = value;
        }

        public NetworkPrefabRef CustomizeItemPrefabRefLobby
        {
            get => customizeItemPrefabRefLobby;
            set => customizeItemPrefabRefLobby = value;
        }

        public GameObject CustomizeItemPrefab
        {
            get => customizeItemPrefab;
            set => customizeItemPrefab = value;
        }

        public GameObject CustomizeItemPrefabLobby
        {
            get => customizeItemPrefabLobby;
            set => customizeItemPrefabLobby = value;
        }

        public float CustomizeItemPrice
        {
            get => customizeItemPrice;
            set => customizeItemPrice = value;
        }

        public float CustomizeInGameSpawnYOffset
        {
            get => customizeInGameSpawnYOffset;
            set => customizeInGameSpawnYOffset = value;
        }

        public bool CustomizeItemIsLocked
        {
            get => customizeItemIsLocked;
            set => customizeItemIsLocked = value;
        }

        public Vector3 CustomizeItemSpawnPosition
        {
            get => customizeItemSpawnPosition;
            set => customizeItemSpawnPosition = value;
        }

        public Vector3 CustomizeItemSpawnRotation => customizeItemSpawnRotation;

        public float CustomizeItemScale => customizeItemScale;
    }
}