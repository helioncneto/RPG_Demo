using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter target;
        Text healthText;
        Health health;
        //float percentage = 100;

        private void Awake()
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
            healthText = GetComponent<Text>();
            
        }

        private void Update()
        {
            health = target.GetTarget();
            if (health == null)
            {
                healthText.text = "N/A";
                
            } else
            {
                //percentage = target.GetTarget().GetPercentage();
                // "{0:0}%" -> Remove decimais
                healthText.text = String.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealthPoints());
            }
            
        }


    }

}