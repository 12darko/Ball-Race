using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Player.NetworkInput
{
    public struct NetworkPlayerInput : INetworkInput
    {
        public Vector3 NetworkInput;
        public Vector3 NetworkLookDelta;
        public Vector3 NetworkCameraLookPosition;
        public Vector3 NetworkCameraForwardPosition;
        public NetworkButtons NetworkAllButtons;
        public Vector3 NetworkAimForward;
        public Vector3 NetworkCameraPosition;
    }
}