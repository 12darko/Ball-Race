using Fusion;
using UnityEngine;

public class LobbyRootParenter : NetworkBehaviour
{
    [SerializeField] private string seatName = "SeatPoint";
    
    [Networked] private NetworkId BoatId { get; set; }
    
    private bool _parented;

    public void SetSeatParent(NetworkId boatId)
    {
        if (!Object.HasStateAuthority) return;
        BoatId = boatId;
        Debug.Log($"[LobbyRootParenter] Root {Object.InputAuthority} -> BoatId set: {boatId}");
    }

    public override void Render()
    {
        if (_parented) return;
        if (BoatId == default) return;

        // Boat'ı bul
        if (!Runner.TryFindObject(BoatId, out NetworkObject boat))
        {
            return;
        }

        Debug.Log($"[LobbyRootParenter] Root {Object.InputAuthority} found Boat {boat.InputAuthority} with Id {BoatId}");

        // Seat'i bul
        Transform seat = FindDeepChild(boat.transform, seatName);
        if (seat == null)
        {
            Debug.LogError($"[LobbyRootParenter] '{seatName}' bulunamadı in boat {boat.InputAuthority}!");
            return;
        }

        // Parent'la
        transform.SetParent(seat, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        
        _parented = true;
        Debug.Log($"[LobbyRootParenter] ✅ Root {Object.InputAuthority} parented to Boat {boat.InputAuthority} seat");
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        if (parent == null) return null;

        for (int i = 0; i < parent.childCount; i++)
        {
            var c = parent.GetChild(i);
            if (c.name == name) return c;

            var r = FindDeepChild(c, name);
            if (r != null) return r;
        }
        return null;
    }
}