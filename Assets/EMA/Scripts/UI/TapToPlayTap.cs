using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EMA.Scripts.UI
{
    public class TapToPlayTap : TapToPlay
    {
        [SerializeField] private Shadow textShadow;
        
        private void Start()
        {
            StartTweens();
        }

        private void StartTweens()
        {
            DOTween.To(() => textShadow.effectDistance, x => textShadow.effectDistance = x, Vector2.zero, 1.5f).
                SetEase(Ease.InOutQuad).SetId("shadow").SetLoops(-1, LoopType.Yoyo);
        }
        
    }
}
