using System;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.Effects.SliderEffect
{
    public class CircularSliderEffect : MonoBehaviour
    {
        [SerializeField] private Transform sliderRotater;

        private void Start()
        {
            sliderRotater.DOLocalRotate(new Vector3(0, 0, 360f), 1.2f, RotateMode.FastBeyond360).SetEase(Ease.OutQuart).SetLoops(-1, LoopType.Incremental);
        }
    }
}