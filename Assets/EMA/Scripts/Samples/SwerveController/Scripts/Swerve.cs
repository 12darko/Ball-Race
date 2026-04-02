using UnityEngine;

namespace EMA.Scripts.Samples.SwerveController.Scripts
{
    public static class Swerve 
    {
        public static void SideMovement(Rigidbody movingPiece,float touchDelta, float moveSpeed)
        {
            var _touchDeltaX = Mathf.Clamp(touchDelta, -.5f, .5f);

           var _inversedVel = movingPiece.transform.InverseTransformDirection(movingPiece.linearVelocity);
           _inversedVel.x = _touchDeltaX * moveSpeed * Time.fixedDeltaTime;
           var _vel = movingPiece.transform.TransformDirection(_inversedVel);
           
            movingPiece.linearVelocity = _vel;
        }
    }
}
