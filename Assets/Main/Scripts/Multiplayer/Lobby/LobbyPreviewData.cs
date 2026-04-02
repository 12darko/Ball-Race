using Fusion;
using Multiplayer.Player;
using UnityEngine;

public class LobbyPreviewData : NetworkBehaviour
{
    [Networked] public int BallIndex { get; set; }
    [Networked] public int HatIndex  { get; set; }
    [Networked] public int FaceIndex { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            RPC_SetIndexes(
                PlayerData.Instance.SelectedBallIndex,
                PlayerData.Instance.SelectedHatIndex,
                PlayerData.Instance.SelectedFaceIndex
            );
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetIndexes(int ball, int hat, int face)
    {
        BallIndex = ball;
        HatIndex  = hat;
        FaceIndex = face;
    }
}