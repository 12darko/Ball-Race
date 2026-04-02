using Fusion;
using Fusion.Addons.Physics;
using Main.Scripts.Player;
using UnityEngine;

public class NetworkRespawnZone : NetworkPlayerZone
{
    protected override void HandlePlayer(NetworkObject player)
    {
        var data = player.GetComponentInChildren<NetworkPlayerRespawnData>();
        if (data == null) return;

        Vector3 targetPos = data.LastSafePosition;
        Quaternion targetRot = data.LastSafeRotation;

        // ✅ Teleport (NetworkRigidbody3D varsa onu kullan)
        var nrb3D = player.GetComponentInChildren<NetworkRigidbody3D>();
        if (nrb3D != null && nrb3D.Rigidbody != null)
        {
            nrb3D.Rigidbody.position = targetPos;
            nrb3D.Rigidbody.rotation = targetRot;

            nrb3D.Rigidbody.linearVelocity = Vector3.zero;
            nrb3D.Rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            // Fallback (NRB yoksa)
            player.transform.SetPositionAndRotation(targetPos, targetRot);

            var rb = player.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // ✅ Respawn sonrası kamera collision'ını kısa süre kapat (owner client'ta)
        var collisionToggle = player.GetComponentInChildren<NetworkRespawnCameraCollisionToggle>();
        if (collisionToggle != null)
            collisionToggle.RPC_DisableCollisionTemporarily();
    }
}