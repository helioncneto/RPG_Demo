using UnityEngine.UI;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform bar;
        [SerializeField] Health healthComponent;
        [SerializeField] Canvas canvas;

        void Update()
        {
            //float fraction = healthComponent.GetHealthFraction();
            // print(fraction);
            if (Mathf.Approximately(healthComponent.GetHealthFraction(), 0)
            ||  Mathf.Approximately(healthComponent.GetHealthFraction(), 1))
            {
                canvas.enabled = false;
                return;
            }

            canvas.enabled = true;
            bar.localScale = new Vector3(healthComponent.GetHealthFraction(), 1f, 1f);
        }
    }

}