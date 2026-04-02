using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection
{
    public abstract class NotificationType : NetworkBehaviour
    {
        [SerializeField] protected NetworkObject     notificationItem;
        [SerializeField] protected NotificationContent notificationContent;

        public abstract void NotificationController();
        public abstract void NotificationSetParent(NetworkObject networkObject);
        public abstract void NotificationSetValue(NotificationItem notificationItem);
    }
}