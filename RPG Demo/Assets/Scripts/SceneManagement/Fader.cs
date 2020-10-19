using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup _canvaGroup;
        Coroutine currentFade = null;

        private void Awake()
        {
            _canvaGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvaGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            
            if (currentFade != null)
            {
                StopCoroutine(currentFade);
            }

            currentFade = StartCoroutine(FadeRoutine(target, time));
            return currentFade;
        }

        IEnumerator FadeRoutine(float target, float time)
        {
            while(!Mathf.Approximately(_canvaGroup.alpha, target))
            {
                _canvaGroup.alpha = Mathf.MoveTowards(_canvaGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}
