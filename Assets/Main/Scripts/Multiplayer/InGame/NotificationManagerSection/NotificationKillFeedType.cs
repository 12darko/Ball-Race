using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection
{
    public class NotificationKillFeedType : NotificationType
    {
        [SerializeField] private float displayDuration = 4f;

        public override void NotificationController() { }

        public void TriggerKillFeed(EliminationData data)
        {
            if (!Runner.IsServer) return;
            SpawnItem(data, $"{data.KillerName} knocked out {data.VictimName}!");
        }

        public void TriggerGoal(EliminationData data)
        {
            if (!Runner.IsServer) return;
            SpawnItem(data, $"GOAL! {data.KillerName} scored!");
        }

        private void SpawnItem(EliminationData data, string message)
        {
            Runner.Spawn(
                notificationItem,
                transform.position,
                Quaternion.identity,
                Object.InputAuthority,
                (runner, o) =>
                {
                    NotificationSetParent(o);
                    var item = o.GetComponent<NotificationItem>();
                    NotificationSetValue(item);

                    var tmp = item.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null) tmp.text = message;

                    StartCoroutine(DestroyAfter(o, displayDuration));
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
            if (item == null) return;
            item.IconImage.sprite = item.OtherIcon;
        }

        private IEnumerator DestroyAfter(NetworkObject o, float t)
        {
            yield return new WaitForSeconds(t);
            if (o != null) Runner.Despawn(o);
        }
    }
}