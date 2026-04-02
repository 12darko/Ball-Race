 using DG.Tweening;
using EMA.Scripts.PatternClasses;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIManager : MonoSingleton<InGameUIManager>
{
    [SerializeField] private CanvasGroup endGameCG;
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private Slider baseHealthBarSlider;

    public Slider BaseHealthBarSlider => baseHealthBarSlider;

    public void UpdateWaveTxt(int currentWave)
    {
        waveTxt.text = "Wave\n" + currentWave.ToString();
    }

    public void EnableEndGameUI()
    {
        endGameCG.gameObject.SetActive(true);
        endGameCG.DOFade(1f, 1f);
    }

    public void RestartScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}