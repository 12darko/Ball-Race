using _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Scores;
using _Main.Scripts.Multiplayer.Multiplayer.InGame.Spectator;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame
{
    public class InGameSystems : MonoBehaviour
    {
       
            [SerializeField] private NotificationManager       notificationManager;
            [SerializeField] private NetworkPlayerEliminationHandler eliminationHandler;
            [SerializeField] private ScoringSystem             scoringSystem;
            [SerializeField] private SpectatorManager          spectatorManager;

            public void Configure(InGameMode mode)
            {
                scoringSystem.SetMode(mode);
                notificationManager.SetMode(mode);

                Debug.Log($"[InGameSystems] Configured for mode: {mode}");
            }
        }
    }
 