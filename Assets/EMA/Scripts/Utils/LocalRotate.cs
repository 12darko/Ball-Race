using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EMA.Scripts.Utils
{
    public class LocalRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 localRotate;
        [SerializeField] private float duration;
        [SerializeField] private Ease tweenEase = Ease.Unset;
        [SerializeField] private bool randomize;

        private Vector3 pos;

        private void Start()
        {
            LocalMoveTween();
        }

        private void LocalMoveTween()
        {
            if (randomize)
                duration = Random.Range(duration - .5f, duration + .5f);
            
            transform.DOLocalRotate(localRotate, duration).SetEase(tweenEase).SetLoops(-1, LoopType.Yoyo);
            
        }
    }
}
