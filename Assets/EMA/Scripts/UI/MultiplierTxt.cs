using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EMA.Scripts.UI
{
    public class MultiplierTxt : MonoBehaviour
    {
        [SerializeField] private Text multiplierTxt;
        [Space][SerializeField] private Color multiplierTweenColor;

        private void OnEnable()
        {
            multiplierTxt.DOColor(multiplierTweenColor, .8f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).SetId("multiplier");
        }
        

        private void OnDisable()
        {
            DOTween.Kill("multiplier");
        }
    }
}
