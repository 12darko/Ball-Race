using System.Collections.Generic;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Fusion;
using UnityEngine;

public class ObstacleManager : NetworkBehaviour
{
    // ─────────────────────────────────────────────────────────────
    // Inspector
    // ─────────────────────────────────────────────────────────────
 
    [Header("Obstacle Listesi")]
    [SerializeField] private List<ObstacleEntry> obstacleEntries = new List<ObstacleEntry>();
 
    [Header("Genel Limit")]
    [Tooltip("Aynı anda maksimum kaç obstacle aktif olabilir. 0 = sınırsız.")]
    [SerializeField] private int maxActiveObstacles = 0;
 
    [Header("Mod Ayarları")]
    [Tooltip("GameModeManager yoksa buradan manuel set edilebilir.")]
    [SerializeField] private InGameMode currentMode = InGameMode.Standard_Mode;
 
    // ─────────────────────────────────────────────────────────────
    // Mod → Davranış Tablosu
    // ─────────────────────────────────────────────────────────────
 
    private struct ModeSettings
    {
        public bool canPush;
        public bool canDamage;
    }
 
    private static readonly Dictionary<InGameMode, ModeSettings> ModeTable =
        new Dictionary<InGameMode, ModeSettings>
    {
        { InGameMode.Standard_Mode,     new ModeSettings { canPush = true, canDamage = false } },
        { InGameMode.Lap_Mode,          new ModeSettings { canPush = true, canDamage = false } },
        { InGameMode.Fall_Balls,        new ModeSettings { canPush = true, canDamage = true  } },
        { InGameMode.Team_Match,        new ModeSettings { canPush = true, canDamage = false } },
        { InGameMode.Team_Soccer_Match, new ModeSettings { canPush = true, canDamage = false } },
    };
 
    // ─────────────────────────────────────────────────────────────
    // Fusion Lifecycle
    // ─────────────────────────────────────────────────────────────
 
    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;
 
        if (obstacleEntries == null || obstacleEntries.Count == 0)
        {
            Debug.LogWarning($"[ObstacleManager] {gameObject.name}: Obstacle listesi boş!");
            return;
        }
 
        TryGetModeFromManager();
 
        List<Obstacle> toKeep   = new List<Obstacle>();
        List<Obstacle> toRemove = new List<Obstacle>();
 
        // 1. Enabled + SpawnChance filtresi
        foreach (ObstacleEntry entry in obstacleEntries)
        {
            if (entry.obstacle == null) continue;
 
            if (!entry.enabled)
            { toRemove.Add(entry.obstacle); continue; }
 
            if (Random.Range(0, 100) < entry.spawnChance)
                toKeep.Add(entry.obstacle);
            else
                toRemove.Add(entry.obstacle);
        }
 
        // 2. MaxActive limiti
        if (maxActiveObstacles > 0 && toKeep.Count > maxActiveObstacles)
        {
            Shuffle(toKeep);
            while (toKeep.Count > maxActiveObstacles)
            {
                toRemove.Add(toKeep[toKeep.Count - 1]);
                toKeep.RemoveAt(toKeep.Count - 1);
            }
        }
 
        // 3. Mod ayarlarını aktiflere uygula
        ModeSettings settings = GetModeSettings();
        foreach (Obstacle obs in toKeep)
        {
            obs.canPushPlayer   = settings.canPush;
            obs.canDamagePlayer = settings.canDamage;
        }
 
        // 4. Fazlaları Despawn
        foreach (Obstacle obs in toRemove)
        {
            if (obs == null) continue;
            NetworkObject no = obs.GetComponent<NetworkObject>();
            if (no != null) Runner.Despawn(no);
            else            Destroy(obs.gameObject);
        }
 
        Debug.Log($"[ObstacleManager] Mod: {currentMode} | " +
                  $"Aktif: {toKeep.Count} | Despawn: {toRemove.Count} | " +
                  $"Push: {settings.canPush} | Damage: {settings.canDamage}");
    }
 
    // ─────────────────────────────────────────────────────────────
    // Public — Dışarıdan mod set etmek için
    // ─────────────────────────────────────────────────────────────
 
    public void SetGameMode(InGameMode mode)
    {
        currentMode = mode;
    }
 
    // ─────────────────────────────────────────────────────────────
    // Yardımcılar
    // ─────────────────────────────────────────────────────────────
 
    private ModeSettings GetModeSettings()
    {
        if (ModeTable.TryGetValue(currentMode, out ModeSettings s)) return s;
        return new ModeSettings { canPush = true, canDamage = false };
    }
 
    /// <summary>
    /// GameModeManager singleton varsa buraya bağla.
    /// Örnek: currentMode = GameModeManager.Instance.CurrentMode;
    /// </summary>
    private void TryGetModeFromManager()
    {
        // TODO: currentMode = GameModeManager.Instance.CurrentMode;
    }
 
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
 
    // ─────────────────────────────────────────────────────────────
    // Editor
    // ─────────────────────────────────────────────────────────────
 
#if UNITY_EDITOR
    [ContextMenu("Refresh Obstacles From Children")]
    private void RefreshObstacles()
    {
        Obstacle[] found = GetComponentsInChildren<Obstacle>(includeInactive: true);
        List<ObstacleEntry> updated = new List<ObstacleEntry>();
 
        foreach (Obstacle obs in found)
        {
            ObstacleEntry existing = obstacleEntries.Find(e => e.obstacle == obs);
            updated.Add(existing ?? new ObstacleEntry
            {
                obstacle    = obs,
                enabled     = true,
                spawnChance = 100
            });
        }
 
        obstacleEntries = updated;
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[ObstacleManager] {found.Length} obstacle bulundu.");
    }
 
    private void Reset() => RefreshObstacles();
#endif
}