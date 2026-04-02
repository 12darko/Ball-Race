using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets._Main.Scripts.UI
{
    public class Transition : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(true);
            var cg = GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.DOFade(0f, 1f).OnComplete(()=> gameObject.SetActive(false));
        }
    }
}