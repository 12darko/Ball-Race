// ==================== GrapplingStop.cs ====================
using System.Collections;
using Fusion;
using Player;
using UnityEngine;

namespace Multiplayer.SkillSystem.SkillTypes.FollowedBuffs
{
    public class GrapplingStop : NetworkBehaviour
    {
        public IEnumerator StopGrapple(
            NetworkPlayers skillUsingPlayer,
            LineRenderer grappleLr,
            GrapplingMoveTest moveTest,
            float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            // FIX: Null kontrolleri eklendi (obje despawn olmuş olabilir)
            if (skillUsingPlayer != null &&
                skillUsingPlayer.networkPlayersManager != null &&
                skillUsingPlayer.networkPlayersManager.NetworkPlayersMovement != null)
            {
                skillUsingPlayer.networkPlayersManager.NetworkPlayersMovement.PlayerIsFreeze = false;
            }

            if (moveTest != null)
                moveTest.Grappling = false;

            if (grappleLr != null)
                grappleLr.enabled = false;
        }
    }
}