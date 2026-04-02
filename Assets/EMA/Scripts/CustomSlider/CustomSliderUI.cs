using System;
using UnityEngine;

namespace EMA.Scripts.CustomSlider
{
    public class CustomSliderUI : MonoBehaviour
    {
        [Header("Comps")][SerializeField] private RectTransform fillMaskRectTr;

        [SerializeField][Range(0f, 1f)] private float sliderValue;

        public float SliderValue {
            get => sliderValue;
            set {
                sliderValue = value;
                SetPropertiesOfSlider();
            }
        }

        private void OnValidate()
        {
            SetPropertiesOfSlider();
        }
        
        private void SetPropertiesOfSlider()
        {
            var _v = sliderValue * Vector3.one;
            _v.y = 1f;
            _v.z = 1f;
            fillMaskRectTr.localScale = _v;
        }
    }
}
