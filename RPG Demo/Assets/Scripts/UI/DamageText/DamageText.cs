﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        //[SerializeField] Text damageText;
        Text damageText;

        private void Awake()
        {
            damageText = GetComponentInChildren<Text>();
        }

        public void SetDamageText(float damage)
        {
            damageText.text = String.Format("{0:0}", damage);
        }
    }

}