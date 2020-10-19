using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        Text healthText;
        //float percentage = 100;

        private void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            healthText = GetComponent<Text>();
            //percentage = health.GetPercentage();
        }

        private void Update()
        {
            //percentage = health.GetPercentage();
            // "{0:0}%" -> Remove decimais
            //healthText.text = String.Format("{0:0}%", percentage);
            healthText.text = String.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealthPoints());
        }


    }

}