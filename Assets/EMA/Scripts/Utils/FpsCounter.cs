using UnityEngine;
using UnityEngine.UI;

namespace EMA.Scripts.Utils
{
    public class FpsCounter : MonoBehaviour
    {
        public Text txt;

        private float pollingTime = 1f;
        private float time;
        private int frameCount;


        void Update()
        {
            time += Time.deltaTime;

            frameCount++;

            if (time >= pollingTime) {
                int frameRate = Mathf.RoundToInt(frameCount / time);
                txt.text = frameRate.ToString();

                time -= pollingTime;
                frameCount = 0;
            }
        }
    }
}
