using DG.Tweening;
using UnityEngine;

namespace Assets._Main.Scripts.UI
{
    public static class ObjectController
    {
        private static Vector2 _startObjectPos;
        private static readonly float _upperYIncrement = 1500;

        public static void SetActiveEffectController(GameObject openGameObject, GameObject closeGameObject, float duration)
        {
            // Aktif animasyon varsa durdur (çakışma önlemi)
            Sequence sequence = DOTween.Sequence();
            DOTween.Kill(sequence);
            bool hasTween = false;

            // Kapanacak obje varsa önce onu kapat
            if (closeGameObject != null && closeGameObject.activeSelf)
            {
                var closeRect = closeGameObject.GetComponent<RectTransform>();
                if (closeRect != null)
                {
                    _startObjectPos = closeRect.position;
                    sequence.Append(
                        closeRect.DOLocalMoveY(_startObjectPos.y +_upperYIncrement, duration)
                            .SetEase(Ease.OutBack)
                            .OnComplete(() => closeGameObject.SetActive(false))
                    );
                    hasTween = true;
                }
            }

            // Açılacak obje varsa sonra onu aç
            if (openGameObject != null)
            {
                var openRect = openGameObject.GetComponent<RectTransform>();
                if (openRect != null)
                {
                    sequence.AppendCallback(() =>
                    {
                        openGameObject.SetActive(true);
                        // Başlangıç pozisyonunu biraz aşağıya koy
                     });

                    sequence.Append(
                        openRect.DOLocalMoveY(0, duration)
                            .SetEase(Ease.InBack)
                    );
                    hasTween = true;
                }
            }

            // Gerçekten bir tween varsa çalıştır
            if (hasTween)
                sequence.Play();
        
        } public static void SetActiveEffectControllerMessageBox(GameObject openGameObject, GameObject closeGameObject, float duration)
        {
            // Aktif animasyon varsa durdur (çakışma önlemi)
            Sequence sequence = DOTween.Sequence();
            DOTween.Kill(sequence);
            bool hasTween = false;

            // Kapanacak obje varsa önce onu kapat
            if (closeGameObject != null && closeGameObject.activeSelf)
            {
                var closeRect = closeGameObject.GetComponent<RectTransform>();
                if (closeRect != null)
                {
                    _startObjectPos = closeRect.position;
                    sequence.Append(
                        closeRect.DOLocalMoveY(_startObjectPos.y +_upperYIncrement, duration)
                            .SetEase(Ease.OutBack)
                            .OnComplete(() => closeGameObject.SetActive(false))
                    );
                    hasTween = true;
                }
            }

            // Açılacak obje varsa sonra onu aç
            if (openGameObject != null)
            {
                var openRect = openGameObject.GetComponent<RectTransform>();
                if (openRect != null)
                {
                    sequence.AppendCallback(() =>
                    {
                        openGameObject.SetActive(true);
                        // Başlangıç pozisyonunu biraz aşağıya koy
                     });

                    sequence.Append(
                        openRect.DOLocalMoveY(0, duration)
                            .SetEase(Ease.InBack)
                    );
                    hasTween = true;
                }
            }

            // Gerçekten bir tween varsa çalıştır
            if (hasTween)
                sequence.Play();
        
        }
    }
}