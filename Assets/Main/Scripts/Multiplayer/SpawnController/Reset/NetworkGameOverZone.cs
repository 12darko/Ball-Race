using Fusion;
using UnityEngine;

public class NetworkGameOverZone : NetworkPlayerZone
{
    protected override void HandlePlayer(NetworkObject player)
    {
        // Basit elimination
        Runner.Despawn(player);

        // İstersen burada:
        // - Score düş
        // - UI event tetikle
        // - Alive = false yap
    }
}