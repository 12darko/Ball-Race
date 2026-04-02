using EMA.Scripts.PatternClasses;
using UnityEngine;
using UnityEngine.Events;

namespace EMA.Scripts.UI
{
    public abstract class TapToPlay : MonoSingleton<TapToPlay>
    {
        [SerializeField] private Canvas canvas;

        public UnityEvent StartGameEvent = new UnityEvent();
        private void OnEnable()
        {
            UIEma.IsGamePaused = true;
        }

        private void OnDisable()
        {
            UIEma.IsGamePaused = false;
        } 

        public void StartTheGame()
        {
            StartGameEvent?.Invoke();
            canvas.gameObject.SetActive(false);
        }
    }
}
