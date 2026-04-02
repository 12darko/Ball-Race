using DG.Tweening;
using UnityEngine;

namespace EMA.Scripts.Utils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAlphaFade : MonoBehaviour
    {
        private SpriteRenderer sprite;
        [SerializeField] private float duration;
        [SerializeField] private float alphaEnd;
        [SerializeField] private Ease ease;
        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            sprite.DOFade(alphaEnd, duration).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        }
    }
}
