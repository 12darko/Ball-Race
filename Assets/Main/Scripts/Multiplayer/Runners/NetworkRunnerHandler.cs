using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Main.Scripts.Multiplayer.FusionCore.StartManager;
using _Main.Scripts.Multiplayer.LevelManager;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;

using _Main.Scripts.Multiplayer.Player;
using _Main.Scripts.Multiplayer.Runners;
using _Main.Scripts.Multiplayer.Singleton;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Player;
using Player.Runner;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    #region Variables & Properties

    [Header("Player Components")]
    [SerializeField] private RunnerPlayers playerPrefab;

    // ❌ Dictionary serialize edilmez - bozuyordu / inspector'a yazılmaz
    private readonly Dictionary<PlayerRef, NetworkObject> _spawnedCharacters =
        new Dictionary<PlayerRef, NetworkObject>();

    [SerializeField] private NetworkPlayerInputManager playerNetworkInputHandler;

    public Dictionary<PlayerRef, NetworkObject> SpawnedCharacters => _spawnedCharacters;

    [Header("Runner Component")]
    [SerializeField] private NetworkRunner networkRunnerPrefab;

    private NetworkRunner _runner;
    public NetworkRunner NetRunner => _runner;

    [Header("Session Component")]
    [SerializeField] private NetworkRunnerListController networkRunnerListController;
    [SerializeField] private SessionMapName mapName;
    [FormerlySerializedAs("modeName")] [SerializeField] private InGameMode inGameModeName;

    public RunnerPlayers PlayerPrefab
    {
        get => playerPrefab;
        set => playerPrefab = value;
    }

    public SessionMapName MapName
    {
         set => mapName = value;
    }

    public InGameMode ModeName
    {
        set => inGameModeName = value;
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        var existingRunner = FindObjectOfType<NetworkRunner>();
        if (existingRunner != null)
        {
            _runner = existingRunner;

            // ✅ runner zaten sahnedeyse callback eklemezsen OnPlayerJoined kaçabiliyor
            _runner.AddCallbacks(this);
        }

        mapName = default(SessionMapName);

        if (mapName is SessionMapName.NONE or (SessionMapName)0)
            mapName = SessionMapName.AUTUMN;
    }

    private void Start()
    {
        // boş
    }

    #endregion

    #region Runner Management (Create & Connect)

    public NetworkRunner GetOrCreateInfo()
    {
        if (_runner == null)
        {
            _runner = Instantiate(networkRunnerPrefab, GlobalManager.Instance.ParentObj.transform);
            _runner.AddCallbacks(this);
            Debug.Log("NetworkRunner oluşturuldu ve Callbackler eklendi.");
        }
        return _runner;
    }

    public void Disconnect()
    {
        if (_runner != null)
        {
            _runner.Shutdown(false, ShutdownReason.Ok, true);
        }
    }

    #endregion

    #region Game Logic (Start, Join, Lobby)

    public async void StartGame(GameMode mode, GameModeCategory category, string sessionName, bool isVisible, bool isOpen,
        int playerCount,string isTeam,  string loadingSuccessMessage, string loadingErrorMessage)
    {
        var runner = GetOrCreateInfo();
        runner.ProvideInput = true;
 
        var sessionProperties = new Dictionary<string, SessionProperty>();
        sessionProperties["LobbyName"] = sessionName.ToString();
        sessionProperties["GameMap"] = mapName.ToString();
        sessionProperties["GameMode"] = inGameModeName.ToString();
        sessionProperties["GameCategory"] = (int)category;
        sessionProperties["TeamMode"] = isTeam;

        
        // ✅ category'ye göre lobby sahnesi
        SceneRef scene = category == GameModeCategory.Fall_Game
            ? SceneRef.FromIndex((int)SceneList.Lobby_FallGame)
            : SceneRef.FromIndex((int)SceneList.Lobby_Arcade);

        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Single);

        var startGameResult = await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            CustomLobbyName = "Lobby",
            IsVisible = isVisible,
            IsOpen = isOpen,
            PlayerCount = playerCount,
            SceneManager = runner.GetComponent<NetworkLevelManager>(),
            MatchmakingMode = MatchmakingMode.FillRoom,
            SessionProperties = sessionProperties
        });

        if (!startGameResult.Ok)
        {
            Debug.LogError("Oyun başlatılamadı!");
            Destroy(runner.gameObject);
            _runner = null;
        }
    }

    public async void RandomStartGame(GameMode mode, GameModeCategory category, bool isVisible, bool isOpen, int playerCount,
        string loadingSuccessMessage, string loadingErrorMessage)
    {
        var runner = GetOrCreateInfo();
        runner.ProvideInput = true;

        var sessionProperties = new Dictionary<string, SessionProperty>();
        sessionProperties["GameCategory"] = (int)category;

        SceneRef scene = category == GameModeCategory.Fall_Game
            ? SceneRef.FromIndex((int)SceneList.Lobby_FallGame)
            : SceneRef.FromIndex((int)SceneList.Lobby_Arcade);

        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Single);

        var startGameResult = await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            Scene = scene,
            CustomLobbyName = "Lobby",
            IsVisible = isVisible,
            IsOpen = isOpen,
            PlayerCount = playerCount,
            SceneManager = runner.GetComponent<NetworkLevelManager>(),
            MatchmakingMode = MatchmakingMode.FillRoom,
            SessionProperties = sessionProperties
        });

        if (!startGameResult.Ok)
        {
            Destroy(runner.gameObject);
            _runner = null;
        }
    }

    public void JoinLobby()
    {
        var runner = GetOrCreateInfo();
        _ = joinLobbyTask(runner);
    }

    private async Task joinLobbyTask(NetworkRunner runner)
    {
        Debug.Log("JoinLobby Started");

        var LobbyId = "Lobby";
        var result = await runner.JoinSessionLobby(SessionLobby.Custom, LobbyId);

        if (!result.Ok)
        {
            Debug.LogError($"Unable To Join Lobby {LobbyId}");
            Destroy(runner.gameObject);
            _runner = null;
        }
        else
        {
            Debug.Log("Join To Lobby Success");
        }
    }

    #endregion

    #region Fusion Callbacks (Events)

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoined: " + player);

        // ✅ Kabuk (RunnerPlayers) spawn SADECE SERVER'DA yapılır
        if (!runner.IsServer)
            return;

        if (playerPrefab == null)
        {
            Debug.LogError("[NetworkRunnerHandler] playerPrefab (RunnerPlayers) assign değil!");
            return;
        }

        if (_spawnedCharacters.ContainsKey(player))
            return;

        // RunnerPlayers prefabı bir NetworkObject prefabı olmalı (Fusion Prefab Table'da kayıtlı)
        var shell = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

        if (shell == null)
        {
            Debug.LogError("[NetworkRunnerHandler] RunnerPlayers spawn FAILED (PrefabTable / NetworkObject kontrol et)!");
            return;
        }

        _spawnedCharacters[player] = shell.Object;
        Debug.Log($"[NetworkRunnerHandler] RunnerPlayers spawned for {player}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // ✅ Despawn sadece server'da
        if (runner.IsServer && _spawnedCharacters.TryGetValue(player, out NetworkObject networkObject) && networkObject != null)
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
            Debug.Log("OnPlayerLeft: " + player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (playerNetworkInputHandler == null && NetworkPlayers.LocalPlayers != null)
            playerNetworkInputHandler = NetworkPlayers.LocalPlayers.GetComponentInChildren<NetworkPlayerInputManager>();

        if (playerNetworkInputHandler != null)
        {
            playerNetworkInputHandler.AccumulatedInput.NetworkInput.Normalize();
            input.Set(playerNetworkInputHandler.AccumulatedInput);
            playerNetworkInputHandler.resetInput = true;
            playerNetworkInputHandler.AccumulatedInput.NetworkLookDelta = default;
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"OnShutdown: {shutdownReason}");

        if (shutdownReason == ShutdownReason.HostMigration)
            return;

        _spawnedCharacters.Clear();

        if (_runner != null && _runner.gameObject != null)
            Destroy(_runner.gameObject);

        _runner = null;

        SceneManager.LoadScene((int)SceneList.MainMenu);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!runner.IsServer) return;

        // ✅ Artık hemen spawn yapmıyoruz, GO'yu bekliyoruz
        StartCoroutine(WaitForGoAndSetup());
    }

    private IEnumerator WaitForGoAndSetup()
    {
        // Barrier sahnede yoksa direkt devam (fallback)
        var barrier = FindObjectOfType<NetworkSceneLoadBarrier>();
        while (barrier != null && !barrier.IsGo)
            yield return null;

        var standardSpawner = FindObjectOfType<_Main.Scripts.Multiplayer.Multiplayer.SpawnController.StandardSpawner>();
        if (standardSpawner != null)
        {
            standardSpawner.SetupGameVisualsForAllPlayers();
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    // --- Boş Callbackler ---
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer"); }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host düştü! Migration başlıyor...");
        MigrateRunner(hostMigrationToken);
    }

    private async void MigrateRunner(HostMigrationToken token)
    {
        if (_runner != null)
        {
            await _runner.Shutdown(destroyGameObject: true, ShutdownReason.HostMigration);
            _runner = null;
        }

        var newRunner = GetOrCreateInfo();
        newRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex((int)mapName);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Single);

        var result = await newRunner.StartGame(new StartGameArgs()
        {
            HostMigrationToken = token,
            Scene = scene,
            SceneManager = newRunner.GetComponent<NetworkLevelManager>(),
            MatchmakingMode = MatchmakingMode.FillRoom,
            GameMode = GameMode.AutoHostOrClient
        });

        if (result.Ok)
        {
            Debug.Log("Migration Başarılı! Oyun devam ediyor.");
        }
        else
        {
            Debug.LogError($"Migration Başarısız: {result.ShutdownReason}");
            SceneManager.LoadScene((int)SceneList.MainMenu);
        }
    }

    #endregion
}
