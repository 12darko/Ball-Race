using _Main.Scripts.Multiplayer.LevelManager.PlatformGenerator;
using Fusion;
using UnityEngine;
using System;
using System.Collections.Generic;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Multiplayer.SkillSystem;

public class NetworkPlatformGenerator : NetworkBehaviour
{
    [Header("Room Property Key")]
    [SerializeField] private string modeKey = "mode";

    [Header("Current Mode (Runtime)")]
    [SerializeField] private InGameMode currentMode;

    [Header("Platform Lists By Mode")]
    [SerializeField] private NetworkPrefabRef[] standardModePrefabs;
    [SerializeField] private NetworkPrefabRef[] lapModePrefabs;
    [SerializeField] private NetworkPrefabRef[] fallBallsPrefabs;
    [SerializeField] private NetworkPrefabRef[] teamMatchPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private Transform platformSpawnPoint;
    [SerializeField] private Vector3 platformScale = Vector3.one * 0.35f;

    [Networked] private NetworkObject SpawnedPlatform { get; set; }

    private PlatformSpawnArea _activeArea;
    public PlatformSpawnArea ActiveArea => _activeArea;
    private bool _spawnInProgress = false;
    private int _spawnCallCount = 0;
    // ----------------------------------------------------

    public override void Spawned()
    {
        if (Runner.IsServer)
            ResolveModeFromSession();
    }

    // ----------------------------------------------------
    // STRING NORMALIZE
    // ----------------------------------------------------

    private static string NormalizeMode(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;

        return s.Trim()
            .Replace(" ", "_")
            .Replace("-", "_")
            .ToLowerInvariant();
    }

    // ----------------------------------------------------
    // STRING → ENUM MAP
    // ----------------------------------------------------

    private static bool TryMapMode(string raw, out InGameMode mode)
    {
        mode = InGameMode.Standard_Mode;

        if (string.IsNullOrWhiteSpace(raw))
            return false;

        // 1) "Namespace.Type.Value" gibi gelirse sadece son parçayı al
        // örn: "InGameMode.Lap_Mode" -> "Lap_Mode"
        string cleaned = raw.Trim();
        int lastDot = cleaned.LastIndexOf('.');
        if (lastDot >= 0 && lastDot < cleaned.Length - 1)
            cleaned = cleaned.Substring(lastDot + 1);

        // 2) normalize
        string key = NormalizeMode(cleaned);

        // 3) mapping
        var map = new Dictionary<string, InGameMode>
        {
            { "standard_mode", InGameMode.Standard_Mode },
            { "standard", InGameMode.Standard_Mode },
            { "default", InGameMode.Standard_Mode },

            { "lap_mode", InGameMode.Lap_Mode },
            { "lap", InGameMode.Lap_Mode },

            { "fall_balls", InGameMode.Fall_Balls },
            { "fallballs", InGameMode.Fall_Balls },

            { "team_match", InGameMode.Team_Match },
            { "team", InGameMode.Team_Match },
        };

        if (map.TryGetValue(key, out mode))
            return true;

        // 4) son fallback: cleaned üzerinden parse
        if (Enum.TryParse(cleaned, true, out InGameMode parsed))
        {
            mode = parsed;
            return true;
        }
        Debug.Log($"[PlatformGenerator] mode raw='{raw}' cleaned='{cleaned}' key='{key}'");

        return false;
    }

    // ----------------------------------------------------
    // SESSION'DAN MODE OKUMA
    // ----------------------------------------------------

    private void ResolveModeFromSession()
    {
        if (Runner?.SessionInfo == null)
            return;

        if (!Runner.SessionInfo.Properties.TryGetValue(modeKey, out var prop) || prop == null)
        {
            Debug.LogWarning($"[PlatformGenerator] Mode property yok. Key={modeKey} -> Standard");
            currentMode = InGameMode.Standard_Mode;
            return;
        }

        // ✅ ÖNEMLİ: ToString() değil, gerçek value!
        string raw;

        // Fusion bazen SessionProperty wrapper döndürür
        if (prop is SessionProperty sp)
            raw = sp.PropertyValue?.ToString();
        else
            raw = prop.ToString();

        Debug.Log($"[PlatformGenerator] mode raw='{raw}' (propType={prop.GetType().Name})");

        if (TryMapMode(raw, out var mapped))
        {
            currentMode = mapped;
            Debug.Log($"[PlatformGenerator] Mode mapped -> {currentMode}");
        }
        else
        {
            Debug.LogWarning($"[PlatformGenerator] Mode map edilemedi: '{raw}' -> Standard");
            currentMode = InGameMode.Standard_Mode;
        }
    }

    // ----------------------------------------------------
    // MODE'E GÖRE LISTE
    // ----------------------------------------------------

    private NetworkPrefabRef[] GetCurrentModeList()
    {
        switch (currentMode)
        {
            case InGameMode.Standard_Mode: return standardModePrefabs;
            case InGameMode.Lap_Mode: return lapModePrefabs;
            case InGameMode.Fall_Balls: return fallBallsPrefabs;
            case InGameMode.Team_Match: return teamMatchPrefabs;
            default: return standardModePrefabs;
        }
    }

    // ----------------------------------------------------
    // PLATFORM SPAWN
    // ----------------------------------------------------



public void Server_SpawnRandomPlatform()
{
    _spawnCallCount++;
    Debug.Log($"[PlatformGenerator] Server_SpawnRandomPlatform called #{_spawnCallCount}");

    if (!Runner || !Runner.IsServer) 
    {
        Debug.LogWarning("[PlatformGenerator] Runner yok veya server değil.");
        return;
    }

    if (_spawnInProgress)
    {
        Debug.LogWarning("[PlatformGenerator] Spawn already in progress, skipping.");
        return;
    }

    _spawnInProgress = true;

    try
    {
        ResolveModeFromSession();

        var list = GetCurrentModeList();
        if (list == null || list.Length == 0)
        {
            Debug.LogError($"[PlatformGenerator] Prefab listesi boş! mode={currentMode}");
            return;
        }

        // log liste durumunu
        for (int i = 0; i < list.Length; i++)
            Debug.Log($"[PlatformGenerator] list[{i}] valid={list[i].IsValid}");

        int index = UnityEngine.Random.Range(0, list.Length);
        var chosen = list[index];

        Debug.Log($"[PlatformGenerator] chosen index={index}, isValid={chosen.IsValid}");

        if (!chosen.IsValid)
        {
            Debug.LogError("[PlatformGenerator] Seçilen prefab INVALID! Spawn iptal.");
            return;
        }

        if (SpawnedPlatform != null)
        {
            Debug.Log($"[PlatformGenerator] Mevcut platform var, despawn ediliyor (obj={SpawnedPlatform.name}).");
            Runner.Despawn(SpawnedPlatform);
            SpawnedPlatform = null;
            _activeArea = null;
        }

        if (!Runner.IsRunning)
        {
            Debug.LogWarning("[PlatformGenerator] Runner henüz running değil. Spawn iptal.");
            return;
        }

        Vector3 pos = platformSpawnPoint ? platformSpawnPoint.position : Vector3.zero;
        Quaternion rot = platformSpawnPoint ? platformSpawnPoint.rotation : Quaternion.identity;

        SpawnedPlatform = Runner.Spawn(chosen, pos, rot, null, (runner, obj) =>
        {
            if (obj == null)
            {
                Debug.LogError("[PlatformGenerator] OnBeforeSpawn callback: obj NULL!");
                return;
            }

            obj.transform.localScale = platformScale;
            _activeArea = obj.GetComponentInChildren<PlatformSpawnArea>(true);
            
            
            // ✅ SADECE BU 2 SATIRI EKLE
       
            Debug.Log($"[PlatformGenerator] Spawn callback obj={obj.name}, activeArea={( _activeArea==null ? "NULL" : _activeArea.name)}");
        });

        if (SpawnedPlatform == null)
            Debug.LogError("[PlatformGenerator] Runner.Spawn returned NULL!");
        else
            Debug.Log($"[PlatformGenerator] Spawn succeeded: {SpawnedPlatform.name}");
    }
    finally
    {
        _spawnInProgress = false;
    }
}

}
