using Fusion;

namespace _Main.Scripts.Multiplayer.Multiplayer.InGame
{
    public struct EliminationData : INetworkStruct
    {
        public PlayerRef Victim;
        public PlayerRef Killer;
        public EliminationType Type;
        public NetworkString<_32> VictimName;
        public NetworkString<_32> KillerName;
     
        
    }
}