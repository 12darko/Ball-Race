using Fusion;

namespace Multiplayer.SkillSystem
{
    public class SkillSpawnPoint : NetworkBehaviour
    {
        [Networked] public NetworkBool IsOccupied { get; set; }
    }
}