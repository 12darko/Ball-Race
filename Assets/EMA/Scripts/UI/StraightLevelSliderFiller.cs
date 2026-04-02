using UnityEngine;
using UnityEngine.UI;

namespace EMA.Scripts.UI
{
    public class StraightLevelSliderFiller : MonoBehaviour
    {
        [HideInInspector] public Transform mainBall;
        [HideInInspector] public Transform endPlatform;

        private float levelDistance = 0f;

        public void FillLevelSlider(Slider levelSlider)
        {
            if(levelDistance == 0f)
                levelDistance = Vector3.Distance(mainBall.transform.position, endPlatform.position);
            
            levelSlider.value = 1 - Vector3.Distance(
                mainBall.transform.position, endPlatform.position) / levelDistance;
        }
    }
}
