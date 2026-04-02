using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame
{
    public class GameSystemsManager : MonoBehaviour
    {
        public static GameSystemsManager Instance { get; private set; }

        public InGameMode CurrentMode { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(NetworkRunner runner)
        {
            ReadModeFromSession(runner);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void ReadModeFromSession(NetworkRunner runner)
        {
            if (runner.SessionInfo.Properties.TryGetValue("GameMode", out var prop))
                CurrentMode = (InGameMode)(int)prop;
            else
                CurrentMode = InGameMode.Standard_Mode;

            Debug.Log($"[GameSystemsManager] Mode: {CurrentMode}");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var systems = FindObjectOfType<InGameSystems>();
            if (systems == null) return;

            systems.Configure(CurrentMode);
        }
    }
}