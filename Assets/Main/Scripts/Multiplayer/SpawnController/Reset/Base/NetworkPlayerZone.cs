using Fusion;
using UnityEngine;

public abstract class NetworkPlayerZone : NetworkBehaviour
{
    [Header("Filter")]
    [SerializeField] protected string playerTag = "Player";

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!IsServerOrMaster()) return;

        var netObj = other.GetComponentInParent<NetworkObject>();
        if (netObj == null) return;

        if (!netObj.CompareTag(playerTag)) return;

        HandlePlayer(netObj);
    }

    /// <summary> Server/master tarafında tetiklenir. </summary>
    protected abstract void HandlePlayer(NetworkObject player);

    /// <summary>
    /// Host/Dedicated: IsServer
    /// Shared: IsSharedModeMasterClient
    /// </summary>
    protected bool IsServerOrMaster()
    {
        if (Runner == null) return false;
        return Runner.IsServer || Runner.IsSharedModeMasterClient;
    }
}