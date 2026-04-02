using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EMA.Scripts.Utils
{
    public class LocalMove : MonoBehaviour
    {
        [SerializeField] private Vector3 localMovePos;
        [SerializeField] private float duration;
        [SerializeField] private Ease tweenEase = Ease.Unset;
        [SerializeField] private bool randomize;
        [SerializeField] private AxisForDoTween axis;

        private Vector3 pos;

        private void Start()
        {
            LocalMoveTween();
        }

        private void LocalMoveTween()
        {
            if (randomize)
                duration = Random.Range(duration - .5f, duration + .5f);


            switch (axis) {

                case AxisForDoTween.X:
                    transform.DOLocalMoveX(localMovePos.x, duration).SetEase(tweenEase).SetLoops(-1, LoopType.Yoyo);
                    break;
                case AxisForDoTween.Y:
                    transform.DOLocalMoveY(localMovePos.y, duration).SetEase(tweenEase).SetLoops(-1, LoopType.Yoyo);
                    break;
                case AxisForDoTween.Z:
                    transform.DOLocalMoveZ(localMovePos.z, duration).SetEase(tweenEase).SetLoops(-1, LoopType.Yoyo);
                    break;
                case AxisForDoTween.All:
                    transform.DOLocalMove(localMovePos, duration).SetEase(tweenEase).SetLoops(-1, LoopType.Yoyo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
        }
    }
}
