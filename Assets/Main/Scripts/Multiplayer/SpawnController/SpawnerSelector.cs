using System;
using _Main.Scripts.Multiplayer.SpawnController;
using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.SpawnController
{
    public class SpawnerSelector : MonoBehaviour
    {
        [Header("Room / Session Property")]
        [Tooltip("Room property key (örn: mode)")]
        [SerializeField] private string modeKey = "mode";

        [Tooltip("Takımlı mod değeri (örn: team)")]
        [SerializeField] private string teamValue = "team";

        [Header("Spawners (BaseSpawner)")]
        [SerializeField] private Spawner standardSpawner;
        [SerializeField] private Spawner teamSpawner;

        [Header("Fallback")]
        [Tooltip("Property yoksa team mi seçilsin?")]
        [SerializeField] private bool defaultToTeam = false;

        private NetworkRunner _runner;
        private bool _selected;

        private void Awake()
        {
            _runner = FindObjectOfType<NetworkRunner>();
        }

        private void Start()
        {
            // SADECE SERVER karar verir
            if (_runner == null || !_runner.IsServer) return;

            // SessionInfo bazen geç gelir
            Invoke(nameof(TrySelect), 0.1f);
            Invoke(nameof(TrySelect), 0.3f);
            Invoke(nameof(TrySelect), 0.6f);
        }

        private void TrySelect()
        {
            if (_selected) return;
            if (_runner == null || !_runner.IsServer) return;

            bool teamMode = ReadTeamMode();
            SelectSpawner(teamMode);
            _selected = true;
        }

        private bool ReadTeamMode()
        {
            try
            {
                if (_runner.SessionInfo.IsValid &&
                    _runner.SessionInfo.Properties != null &&
                    _runner.SessionInfo.Properties.TryGetValue(modeKey, out var prop))
                {
                    var value = prop.ToString(); // sende şu an böyle
                    Debug.Log($"[SpawnerSelector] {modeKey}={value}");

                    return value.Equals(teamValue, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SpawnerSelector] ReadTeamMode exception: {e.Message}");
            }

            return defaultToTeam;
        }


        private void SelectSpawner(bool teamMode)
        {
            // Önce hepsini kapat
            SetActive(standardSpawner, false);
            SetActive(teamSpawner, false);

            if (teamMode)
            {
                SetActive(teamSpawner, true);
                teamSpawner.SetupGameVisualsForAllPlayers();
                Debug.Log("[SpawnerSelector] TEAM spawner seçildi");
            }
            else
            {
                SetActive(standardSpawner, true);
                standardSpawner.SetupGameVisualsForAllPlayers();
                Debug.Log("[SpawnerSelector] STANDARD spawner seçildi");
            }
        }

        private void SetActive(Spawner spawner, bool active)
        {
            if (spawner == null) return;
            spawner.enabled = active;

            // İstersen tamamen kapat:
            // spawner.gameObject.SetActive(active);
        }
    }
}
