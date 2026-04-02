 
using Fusion;
using Player;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection
{
    public class NotificationWinType : NotificationType
    {
       [UnitySerializeField] public FinishLine FinishLine { get; set; }

        public override void Spawned()
        {
            if (notificationContent == null)
                notificationContent = FindObjectOfType<NotificationContent>();

            if (FinishLine != null)
                FinishLine.OnPlayerFinished += HandlePlayerFinished;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (FinishLine != null)
                FinishLine.OnPlayerFinished -= HandlePlayerFinished;
        }

        private void HandlePlayerFinished(int position, string playerName)
            => NotificationController();

        public override void NotificationController()
        {
            if (!Runner.IsServer) return;

            Runner.Spawn(
                notificationItem,
                transform.position,
                Quaternion.identity,
                Object.InputAuthority,
                (runner, o) =>
                {
                    NotificationSetParent(o);
                    NotificationSetValue(o.GetComponent<NotificationItem>());
                });
        }

        public override void NotificationSetParent(NetworkObject networkObject)
        {
            if (networkObject == null || notificationContent == null) return;
            networkObject.transform.SetParent(notificationContent.transform);
            networkObject.transform.localScale = Vector3.one;
        }

        public override void NotificationSetValue(NotificationItem item)
        {
            if (item == null || FinishLine == null) return;

            if (!string.IsNullOrEmpty(FinishLine.FirstPlayer.Value))
                item.IconImage.sprite = item.FirstIcon;
            else if (!string.IsNullOrEmpty(FinishLine.SecondPlayer.Value))
                item.IconImage.sprite = item.SecondIcon;
            else if (!string.IsNullOrEmpty(FinishLine.ThirdPlayer.Value))
                item.IconImage.sprite = item.ThirdIcon;
            else
                item.IconImage.sprite = item.OtherIcon;
        }
    }

}