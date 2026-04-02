using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EMA.Scripts.UI
{
    public class LevelProgressUI : MonoBehaviour
    {
        [SerializeField] private Text levelTxt;
        
        [SerializeField] private Slider levelFill;

        public UnityEvent<Slider> LevelFillEvent;
        

        private void Start()
        {
            levelTxt.text = (SceneManager.GetActiveScene().buildIndex).ToString();
        }

        public void DisappearLevelPanel()
        {
            transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InOutBack);
        }

        private void Update()
        {
            LevelFillEvent?.Invoke(levelFill);
        }

    }
}
