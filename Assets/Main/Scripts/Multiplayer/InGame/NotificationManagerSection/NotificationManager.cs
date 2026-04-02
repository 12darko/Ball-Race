using System;
using System.Linq;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection
{
 public class NotificationManager : NetworkBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        private InGameMode _mode = InGameMode.Standard_Mode;

        private void Awake() => Instance = this;

        public void SetMode(InGameMode mode) => _mode = mode;

        public void AddNotificationItem(NotificationSelectType type,
                                        EliminationData data = default)
        {
            var resolved = ResolveType(type);
            if (resolved == null) return;

            switch (resolved.Value)
            {
                case NotificationSelectType.Win:
                    GetComponentInChildren<NotificationWinType>(true)
                        ?.NotificationController();
                    break;

                case NotificationSelectType.Falling:
                    GetComponentInChildren<NotificationFallingType>(true)
                        ?.TriggerFalling(data);
                    break;

                case NotificationSelectType.KillFeed:
                    GetComponentInChildren<NotificationKillFeedType>(true)
                        ?.TriggerKillFeed(data);
                    break;

                case NotificationSelectType.Goal:
                    GetComponentInChildren<NotificationKillFeedType>(true)
                        ?.TriggerGoal(data);
                    break;
            }
        }

        /// Moda göre bildirim tipini filtrele / dönüştür
        private NotificationSelectType? ResolveType(NotificationSelectType requested)
        {
            switch (_mode)
            {
                case InGameMode.Standard_Mode:
                case InGameMode.Lap_Mode:
                    // Kill feed gösterme — sadece falling ve win
                    if (requested == NotificationSelectType.KillFeed)
                        return NotificationSelectType.Falling;
                    return requested;

                case InGameMode.Fall_Balls:
                    // Win bildirimi yok — sadece kill feed ve falling
                    if (requested == NotificationSelectType.Win)
                        return null;
                    return requested;

                case InGameMode.Team_Match:
                    // Standart gibi ama takım win bildirimi — şimdilik aynı
                    return requested;

                case InGameMode.Team_Soccer_Match:
                    // Sadece gol ve falling — win/killfeed yok
                    if (requested == NotificationSelectType.Win ||
                        requested == NotificationSelectType.KillFeed)
                        return null;
                    return requested;

                default:
                    return requested;
            }
        }
    }
  
}