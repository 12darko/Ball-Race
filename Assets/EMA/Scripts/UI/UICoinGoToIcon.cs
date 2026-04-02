using System.Collections.Generic;
using DG.Tweening;
using EMA.Scripts.PatternClasses;
using UnityEngine;
using UnityEngine.UI;

namespace EMA.Scripts.UI
{
    public class UICoinGoToIcon : MonoBehaviour
    {
        [SerializeField] private RectTransform mainCanvas;

        [SerializeField] private ObjectPool coinUIObjectPool;
        [SerializeField] private Transform coinObjectPoolParent;
        [SerializeField] private Transform icon;
        
        private Queue<GameObject> coinUIQueue = new Queue<GameObject>();

        private void Start()
        {
            coinUIObjectPool.CreatePool();
        }

        public void GenerateCoinUIAndMoveIt(Vector3 pos)
        {
            var _camera = Camera.main;

            var _viewportPosition=_camera.WorldToViewportPoint(pos);

            var _sizeDelta = mainCanvas.sizeDelta;
            var _worldObjectScreenPosition=new Vector2(
                 ((_viewportPosition.x*_sizeDelta.x)-(_sizeDelta.x*0.5f)),
                 ((_viewportPosition.y*_sizeDelta.y)-(_sizeDelta.y*0.5f)));
             
 
             var _obj = coinUIObjectPool.GetPooledObject();
             var _rect = _obj.GetComponent<RectTransform>();
             _rect.SetParent(mainCanvas);
            _rect.anchoredPosition = _worldObjectScreenPosition;
             _rect.SetParent(icon);
             _rect.DOLocalMove(Vector3.zero, .5f).OnComplete(TakeCoinUI);
             coinUIQueue.Enqueue(_obj);
        }
        
        private void TakeCoinUI()
        {
            coinUIObjectPool.SendObjectToPool(coinUIQueue.Dequeue()); 
            DOTween.Complete("coinUI");
            icon.DOScale(icon.localScale * 1.3f, .2f).SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad).SetId("coinUI");
        }
    }
}
