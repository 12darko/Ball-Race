using UnityEngine;

namespace EMA.Scripts.Samples.Runner.Scripts
{
    public static class Movement
    {
        public static void NonStopRunning(Rigidbody rb, float moveSpeed)
        {
            
            var _inversedVel = rb.transform.InverseTransformDirection(rb.linearVelocity);
            _inversedVel.z =  moveSpeed * Time.fixedDeltaTime;
            var _vel = rb.transform.TransformDirection(_inversedVel);
           
            rb.linearVelocity = _vel;
        }
    }
}
