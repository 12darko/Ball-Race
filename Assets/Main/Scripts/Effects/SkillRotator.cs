using System;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Effects
{
    public class SkillRotator : MonoBehaviour
    {
        [SerializeField] private Transform rotatorObject;
        [SerializeField] private float duration;

        private void Start()
        {
            rotatorObject.DOLocalRotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
    }
}