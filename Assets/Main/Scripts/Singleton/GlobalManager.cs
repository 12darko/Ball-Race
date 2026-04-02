using UnityEngine;

namespace _Main.Scripts.Multiplayer.Singleton
{
    public class GlobalManager : MonoBehaviour
    {
        public static GlobalManager Instance { get; private set; }

        [Header("References")]
        // DontDestroyOnLoad olacak ana kapsayıcı obje
        [SerializeField] private GameObject parentObj; 
        
        // Editor'den NetworkRunnerHandler prefabını buraya sürükle
        [SerializeField] private NetworkRunnerHandler networkRunnerHandlerPrefab;

        // DÜZELTME BURADA: İsmini diğer kodlarının aradığı gibi "NetworkRunnerHandler" yaptık.
        public NetworkRunnerHandler NetworkRunnerHandler { get; private set; }

        public GameObject ParentObj => parentObj;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                if (parentObj == null) parentObj = gameObject;

                DontDestroyOnLoad(parentObj);

                CreateRunnerHandler();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void CreateRunnerHandler()
        {
            // Handler'ı oluşturup property'e atıyoruz
            NetworkRunnerHandler = Instantiate(networkRunnerHandlerPrefab, parentObj.transform);
            
            Debug.Log("GlobalManager: NetworkRunnerHandler oluşturuldu.");
        }
    }
}