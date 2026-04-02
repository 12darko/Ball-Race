using System;
using System.Linq;
using UnityEngine;

namespace Assets._Main.Scripts.UI.Credits
{
    public class CreditsScroll : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        [Header("Variables")] [SerializeField] private float scrollSpeed = 40f;
        private float _scrollYVariable = 0;

        private void Start()
        {
            _scrollYVariable = rectTransform.anchoredPosition.y;
        }
 

        private void OnDisable()
        {
            rectTransform.anchoredPosition = new Vector2(0, _scrollYVariable);
        }

        private void Update()
        {
            if (rectTransform.gameObject.activeSelf)
            {
                rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
            }
        }
    }
}