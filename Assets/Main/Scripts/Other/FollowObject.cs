using System;
using Fusion;

using UnityEngine;

namespace Main.Scripts.Other
{
    public abstract class FollowObject : NetworkBehaviour
    {
        [SerializeField] protected Transform targetTransform;

        public Transform TargetTransform => targetTransform;


        public abstract void SetTargetTransform(Transform targetTransform);
        
    }
}