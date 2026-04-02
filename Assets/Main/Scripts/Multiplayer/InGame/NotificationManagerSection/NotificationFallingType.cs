using System.Collections;
using System.Collections.Generic;
using Fusion;
using Player;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame.NotificationManagerSection
{
    public class NotificationFallingType : NotificationType
    {
        [SerializeField] private float displayDuration = 3f;

        public override void NotificationController()
        {
        }

        public void TriggerFalling(EliminationData data)
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
                    var item = o.GetComponent<NotificationItem>();
                    NotificationSetValue(item, data);
                    StartCoroutine(DestroyAfter(o, displayDuration));
                });
        }

        public override void NotificationSetParent(NetworkObject networkObject)
        {
            if (networkObject == null || notificationContent == null) return;
            var t = networkObject.transform;
            t.SetParent(notificationContent.transform);
            t.localScale = Vector3.one;
        }

        public override void NotificationSetValue(NotificationItem item)
            => NotificationSetValue(item, default);

        private void NotificationSetValue(NotificationItem item, EliminationData data)
        {
            if (item == null) return;
            item.IconImage.sprite = item.OtherIcon;

            var tmp = item.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
                tmp.text = $"{data.VictimName} fell!";
        }

        private IEnumerator DestroyAfter(NetworkObject o, float t)
        {
            yield return new WaitForSeconds(t);
            if (o != null) Runner.Despawn(o);
        }
    }
}