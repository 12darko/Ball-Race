using System;
using System.Collections;
using TMPro;
 
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Other.LoadinManagers
{
    public class NetworkLoadingUIControllers : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private TMP_Text tipText;
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private Animator characterAnimator;

        [SerializeField] private Vector2 characterAnimStartPos;
        [SerializeField] private Vector2 characterAnimEndPos;


        [SerializeField] private string[] randomTips =
        {
        };

        private NetworkLoadingManagers loadingManager;
        private static readonly int Running = Animator.StringToHash("Running");

        public void Init(NetworkLoadingManagers manager)
        {
            loadingManager = manager;
            StartCoroutine(FadeIn());
            StartCoroutine(SimulateLoading());
        }

        private IEnumerator FadeIn()
        {
            canvasGroup.alpha = 0;
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime * 1.5f;
                yield return null;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                characterAnimator.SetBool(Running, true);
            }
        }

        private IEnumerator SimulateLoading()
        {
            tipText.text = randomTips[Random.Range(0, randomTips.Length)];
            float progress = 0f;
            if (characterAnimator != null)
            {
                // Başta koşma animasyonu başlasın
                characterAnimator.SetBool(Running, true);
            }

            while (progress < 1f)
            {
                progress += Time.deltaTime * 0.25f;
                progressBar.value = progress;

                if (characterAnimator != null)
                {
                    // Karakteri bar boyunca hareket ettir
                    characterAnimator.transform.localPosition = Vector3.Lerp(
                        characterAnimStartPos,
                        characterAnimEndPos,
                        progress
                    );
                }

                loadingText.text = $"Yükleniyor... %{Mathf.RoundToInt(progress * 100)}";
               // loadingManager.ReportProgressServerRpc(progress);
                yield return null;
            }

            // Bar tamamlandı → idle animasyona geç
            if (characterAnimator != null)
            {
                characterAnimator.SetBool(Running, false);
            }

            loadingText.text = "Hazır! Bekleniyor...";
        }
    }
}