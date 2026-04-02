using _Main.Scripts.Multiplayer.Multiplayer.Obstacles;
using UnityEngine;

/// <summary>
/// PressurePlate child objesine ekle.
/// Is Trigger = TRUE — top içinden geçer, geri itilmez.
/// </summary>
public class CannonPressurePlate : MonoBehaviour
{
    [SerializeField] private string ballTag = "Player";

    private CannonObstacle _cannon;
    private bool   _triggered = false;

    private void Awake()
    {
        _cannon = GetComponentInParent<CannonObstacle>();
        if (_cannon == null)
            Debug.LogError("[PressurePlate] Parent'ta Cannon bulunamadı!");

        // ✅ Trigger — top geri itilmez
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag(ballTag)) return;

        Debug.Log("[PressurePlate] ✅ Tetiklendi!");
        _triggered = true;
        _cannon?.OnBallEntered(other);
    }

    public void Reset() => _triggered = false;
}