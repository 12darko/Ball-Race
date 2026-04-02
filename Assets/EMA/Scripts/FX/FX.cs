using System.Collections;
using UnityEngine;

namespace EMA.Scripts.FX
{
    public static class FX
    {
        public static IEnumerator CallFX(GameObject fx, Transform posTr, bool snapping = false)
        {
            var particle = fx.GetComponent<ParticleSystem>();

            var duration = particle.main.duration;

            fx.SetActive(true);
            fx.transform.position = posTr.position;
            fx.GetComponent<ParticleSystem>().Play();
            
            if (snapping)
            {
                duration += Time.time;
                while (Time.time < duration)
                {
                    fx.transform.position = posTr.position;
                    yield return null;
                }
                fx.SetActive(false);
            }

            else
            {
                yield return new WaitForSeconds(duration);
                fx.SetActive(false);
            }
        }

    }
}
