using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup _canvaGroup;

        private void Awake()
        {
            _canvaGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvaGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            while(_canvaGroup.alpha < 1)
            {
                _canvaGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            while (_canvaGroup.alpha > 0)
            {
                _canvaGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }

        
    }
}
