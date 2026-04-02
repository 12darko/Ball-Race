using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EMA.Scripts.UI
{
    public class TapToPlaySwerve : TapToPlay
    {
        [SerializeField] private Slider fingerSlider;

        [SerializeField] private Shadow textShadow;

        private void Start()
        {
            StartTweens();
        }

        private void OnDisable()
        {
            DOTween.Kill("finger");
            DOTween.Kill("shadow");
        }

        private void Update()
        {
            var _dir = Joystick.Instance.Direction;
            if(_dir.magnitude > .05f)
                StartTheGame();
        }

        private void StartTweens()
        {
            fingerSlider.DOValue(1f, 1.2f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).SetId("finger");
            DOTween.To(() => textShadow.effectDistance, x => textShadow.effectDistance = x, Vector2.zero, 1.5f).
                SetEase(Ease.InOutQuad).SetId("shadow").SetLoops(-1, LoopType.Yoyo);
        }
        
    }
}
