// ==================== GrapplingExecute.cs ====================
using System.Collections;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.FollowedBuffs
{
    public class GrapplingExecute : NetworkBehaviour
    {
        [SerializeField] private GrapplingMovementHandler grapplingMovementHandler;
        [SerializeField] private GrapplingStop grapplingStop;

        [Networked] private Vector3 LowestPoint { get; set; }
        [Networked] private float GrapplePointRelativeYPos { get; set; }
        [Networked] private float HighestPointOnArc { get; set; }

        public IEnumerator ExecuteGrapple(
            NetworkPlayers skillUsingPlayer,
            GrapplingMoveTest moveTest,
            float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            if (skillUsingPlayer == null || moveTest == null) yield break;

            skillUsingPlayer.networkPlayersManager.NetworkPlayersMovement.PlayerIsFreeze = false;

            LowestPoint = new Vector3(
                moveTest.transform.position.x,
                moveTest.transform.position.y - 1f,
                moveTest.transform.position.z);

            GrapplePointRelativeYPos = moveTest.GrapplePoint.y - LowestPoint.y;
            HighestPointOnArc = GrapplePointRelativeYPos + moveTest.OverShootYAxis;

            if (GrapplePointRelativeYPos < 0)
                HighestPointOnArc = moveTest.OverShootYAxis;

            grapplingMovementHandler.JumpToPosition(moveTest.GrapplePoint, HighestPointOnArc, skillUsingPlayer);

            StartCoroutine(grapplingStop.StopGrapple(
                moveTest.SkillUsingPlayer,
                moveTest.GrappleLr,
                moveTest,
                1f));
        }
    }
}