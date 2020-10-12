using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        Text healthText;
        private void Awake()
        {
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
            healthText = GetComponent<Text>();
        }

        private void Update()
        {
            healthText.text = String.Format("{0:0}", experience.GetExperiencePoints());
        }
    }
}
