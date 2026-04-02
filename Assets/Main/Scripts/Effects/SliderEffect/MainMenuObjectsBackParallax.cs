using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Effects.SliderEffect
{
    public class MainMenuObjectsBackParallax : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private float xAxis;
        [SerializeField] private float yAxis;

        private void Update()
        {
            rawImage.uvRect = new Rect(rawImage.uvRect.position + new Vector2(xAxis, yAxis) * Time.deltaTime,
                rawImage.uvRect.size);
        }
    }
}