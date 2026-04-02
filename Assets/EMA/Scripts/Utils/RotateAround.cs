using System;
using DG.Tweening;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EMA.Scripts.Utils
{
    public class RotateAround : NetworkBehaviour
    {
        [SerializeField, Networked] private Vector3 rotatePos { get; set; }
        [SerializeField, Networked] private float duration { get; set; }

        [SerializeField, Networked] private NetworkBool randomizeDuration { get; set; }
        [SerializeField, Networked] private NetworkBool randomizePos { get; set; }


        public override void Spawned()
        {
            base.Spawned();
            if (randomizeDuration)
                duration = Random.Range(duration - 1f, duration + 1f);
            if (randomizePos && MyShortcuts.MyShortcuts.GetRandomBoolByChance(.5f))
                rotatePos *= -1f;

            RPC_Rotate();
        }

        private void OnDestroy()
        {
            DOTween.Kill(transform);
        }


        [Rpc]
        private void RPC_Rotate()
        {
            transform.DOLocalRotate(transform.localRotation.eulerAngles + (rotatePos * 360f), duration,
                RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }
 
    }
}