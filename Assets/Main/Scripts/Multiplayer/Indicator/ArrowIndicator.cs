using System;
using DG.Tweening;
using Fusion;
using UnityEngine;

namespace _Main.Scripts.Multiplayer.Multiplayer.Indicator
{
    public class ArrowIndicator : NetworkBehaviour
    {
        [SerializeField] private Transform indicatorObject;
        [SerializeField] private float indicatorYMoveValue;
        [SerializeField] private float indicatorSpeed;
        private void Start()
        {
            indicatorObject.DOLocalMoveY(indicatorYMoveValue, indicatorSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }
}