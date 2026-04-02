using DG.Tweening;
using UnityEngine;

namespace EMA.Scripts.SceneTransitions
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeTransition : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private CanvasGroup cg;

        [SerializeField] private bool playOnAwake;
        private void Start()
        {
            if (playOnAwake)
                ExecuteBlackToTransparent();
        }

        public void ExecuteBlackToTransparent()
        {
            canvas.SetActive(true);
            cg.alpha = 1f;
            cg.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() => canvas.SetActive(false));
        }
        public void ExecuteTransparentToBlack()
        {
            canvas.SetActive(true);
            cg.alpha = 0f;
            cg.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() => canvas.SetActive(false));
        }
    }
}
