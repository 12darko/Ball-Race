using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;

/// <summary>
/// Gemi animasyon sistemi - Özel rota takip edilerek döngüsel hareket
/// </summary>
public class ShipPathAnimation : NetworkBehaviour
{
    [Header("References")]
    public Transform shipRoot;
    public Transform shipMesh;

    [Header("Path")]
    public Transform[] pathPoints;
    public float speed = 3f;
    public float reachDistance = 0.3f;

    [Header("Rotation")]
    public float rotationSpeed = 4f; // derece hissi

    [Header("Visual Motion (Local)")]
    public float waveHeight = 0.15f;
    public float waveSpeed = 1.5f;
    public float rollAngle = 4f;
    public float rollSpeed = 1.2f;

    private int index;
    private float baseMeshY;

    public override void Spawned()
    {
        if (shipRoot == null)
            shipRoot = transform;

        baseMeshY = shipMesh.localPosition.y;
    }

    public override void FixedUpdateNetwork()
    {
        // 🔥 SADECE STATE AUTHORITY HAREKET ETTİRİR
        if (!Object.HasStateAuthority)
            return;

        if (pathPoints == null || pathPoints.Length == 0)
            return;

        MoveAlongPath();
    }

    void Update()
    {
        // Görsel efektler HER CLIENT'TA çalışır
        ApplyVisualMotion();
    }

    void MoveAlongPath()
    {
        Vector3 target = pathPoints[index].position;
        target.y = shipRoot.position.y;

        Vector3 targetDir = (target - shipRoot.position).normalized;

        // Rotation (visual + smooth)
        if (targetDir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(targetDir, Vector3.up);
            shipRoot.rotation = Quaternion.Slerp(
                shipRoot.rotation,
                lookRot,
                rotationSpeed * Runner.DeltaTime
            );
        }

        // 🔥 MOVE = TARGET DIRECTION
        shipRoot.position = Vector3.MoveTowards(
            shipRoot.position,
            target,
            speed * Runner.DeltaTime
        );

        if (Vector3.Distance(shipRoot.position, target) <= reachDistance)
        {
            index++;
            if (index >= pathPoints.Length)
                index = 0;
        }
    }

    void ApplyVisualMotion()
    {
        if (shipMesh == null) return;

        Vector3 pos = shipMesh.localPosition;
        Vector3 rot = shipMesh.localEulerAngles;

        pos.y = baseMeshY + Mathf.Sin(Time.time * waveSpeed) * waveHeight;
        rot.z = Mathf.Sin(Time.time * rollSpeed) * rollAngle;

        shipMesh.localPosition = pos;
        shipMesh.localEulerAngles = rot;
    }
}